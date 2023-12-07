using System.Collections.Generic;

namespace LoxInterpreter
{
    public abstract class Stmt
    {
        public interface Visitor {
            void VisitExpressionStmt(Expression Stmt);
            void VisitPrintStmt(Print Stmt);
            void VisitVarStmt(Var stmt);
            void VisitBlockStmt(Block stmt);
            void VisitIfStmt(If stmt);
            void VisitWhileStmt(While stmt);
        }

        public abstract void Accept(Visitor visitor);

        public class Expression : Stmt
        {
            public readonly Expr expression;

            public Expression(Expr expression)
            {
                this.expression = expression;
            }

            public override void Accept(Stmt.Visitor visitor)
            {
                visitor.VisitExpressionStmt(this);
            }
        }

        public class Print : Stmt
        {
            public readonly Expr expression;
            
            public Print(Expr expression)
            {
                this.expression = expression;
            }

            public override void Accept(Stmt.Visitor visitor)
            {
                visitor.VisitPrintStmt(this);
            }
        }

        public class Var : Stmt
        {
            public readonly Token name;
            public readonly Expr initializer;

            public Var(Token name, Expr initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }

            public override void Accept(Visitor visitor)
            {
                visitor.VisitVarStmt(this);
            }
        }

        public class Block : Stmt
        {
            public readonly List<Stmt> statements;

            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            public override void Accept(Visitor visitor)
            {
                visitor.VisitBlockStmt(this);
            }
        }

        public class If : Stmt
        {
            public readonly Expr condition;
            public readonly Stmt thenBranch;
            public readonly Stmt elseBranch;

            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            public override void Accept(Visitor visitor)
            {
                visitor.VisitIfStmt(this);
            }
        }

        public class While : Stmt
        {
            public readonly Expr condition;
            public readonly Stmt body;

            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }

            public override void Accept(Visitor visitor)
            {
                visitor.VisitWhileStmt(this);
            }
        }
    }
}