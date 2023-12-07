using System;
using System.Collections;
using System.Collections.Generic;

namespace LoxInterpreter
{
    public class Interpreter : Expr.Visitor<object>, Stmt.Visitor
    {
        public readonly Environment globals = new Environment();
        private Environment environment;
        private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();


        public Interpreter()
        {
            environment = globals;
            globals.Define("clock", new ClockFunction());
        }

        private class ClockFunction : LoxCallable
        {
            public int Arity() => 0;

            public object Call(Interpreter interpreter, List<object> arguments)
            {
                return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
            }

            public override string ToString() => "<native fn>";
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return obj.ToString();
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            object left = Evaluate(expr.left);

            if (expr.operatorToken.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.right);
        }

        public object VisitSetExpr(Expr.Set expr)
        {
            object obj = Evaluate(expr.obj);

            if (!(obj is LoxInstance))
            {
                throw new RuntimeError(expr.name, "Only instances have fields.");
            }

            object value = Evaluate(expr.value);
            ((LoxInstance)obj).Set(expr.name, value);
            return value;
        }

        public object VisitThisExpr(Expr.This expr)
        {
            return LookUpVariable(expr.keyword, expr);
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.OperatorToken.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.OperatorToken, right);
                    return -(double)right;
                default:
                    throw new RuntimeError(expr.OperatorToken, "Unsupported unary operator.");
            }
        }

        private void CheckNumberOperand(Token oper, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(oper, "Operand must be a number.");
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public void Resolve(Expr expr, int depth)
        {
            locals[expr] = depth;
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
        }

        public void VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
        }

        public void VisitClassStmt(Stmt.Class stmt)
        {
            environment.Define(stmt.name.Lexeme, null);
            Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>();
            foreach (Stmt.Function method in stmt.methods)
            {
                LoxFunction function = new LoxFunction(method, environment, method.name.Lexeme.Equals("init"));
                methods[method.name.Lexeme] = function;
            }

            LoxClass klass = new LoxClass(stmt.name.Lexeme, methods);
            environment.Assign(stmt.name, klass);
        }

        public void VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
        }

        public void VisitIfStmt(Stmt.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                Execute(stmt.elseBranch);
            }
        }

        public void VisitFunctionStmt(Stmt.Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, environment, false);
            environment.Define(stmt.name.Lexeme, function);
        }

        public void VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
        }

        public void VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.value != null) value = Evaluate(stmt.value);

            throw new Return(value);
        }

        public void VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.Lexeme, value);
        }

        public void VisitWhileStmt(Stmt.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            object value = Evaluate(expr.value);

            int distance = locals[expr];
            if (distance != null)
            {
                environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                globals.Assign(expr.name, value);
            }
            
            return value;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.Name);
        }

        private object LookUpVariable(Token name, Expr expr)
        {
            int distance = locals[expr];
            if (distance != null)
            {
                return environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return globals.Get(name);
            }
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.OperatorToken.Type)
            {
                case TokenType.GREATER:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    throw new RuntimeError(expr.OperatorToken, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double)left * (double)right;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
                default:
                    throw new RuntimeError(expr.OperatorToken, "Unsupported binary operator.");
            }
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            object callee = Evaluate(expr.callee);

            List<object> arguments = new List<object>();
            foreach (Expr argument in expr.arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is LoxCallable))
            {
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            }

            LoxCallable function = (LoxCallable)callee;

            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expr.paren, "Expected" +
                    function.Arity() + " arguments but got " + 
                    arguments.Count + ".");
            }

            return function.Call(this, arguments);
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            object obj = Evaluate(expr.obj);
            if (obj is LoxInstance)
            {
                return ((LoxInstance) obj).Get(expr.name);
            }

            throw new RuntimeError(expr.name, "Only instances have properties.");
        }

        private void CheckNumberOperands(Token oper, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(oper, "Operands must be numbers.");
        }

        
    }
}