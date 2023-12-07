namespace LoxInterpreter
{
    public abstract class Expr
    {
        public interface Visitor<R>
        {
            R VisitBinaryExpr(Binary expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitUnaryExpr(Unary expr);
            R VisitVariableExpr(Variable expr);
            R VisitAssignExpr(Assign expr);
            R VisitLogicalExpr(Logical expr);
            R VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R VisitSetExpr(Set expr);
            R VisitThisExpr(This expr);
            R VisitSuperExpr(Super expr);
        }

        public abstract R Accept<R>(Visitor<R> visitor);

        public class Binary : Expr
        {
            public Expr Left { get; }
            public Token OperatorToken { get; }
            public Expr Right { get; }

            public Binary(Expr left, Token operatorToken, Expr right)
            {
                Left = left;
                OperatorToken = operatorToken;
                Right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr expression { get; }

            public Grouping(Expr expression)
            {
                this.expression = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        public class Literal : Expr
        {
            public object Value { get; }

            public Literal(object value)
            {
                Value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public class Unary : Expr
        {
            public Token OperatorToken { get; }
            public Expr right { get; }

            public Unary(Token operatorToken, Expr right)
            {
                OperatorToken = operatorToken;
                this.right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public class Variable : Expr
        {
            public readonly Token name;

            public Variable(Token name)
            {
                this.name = name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

        public class Assign : Expr
        {
            public readonly Token name;
            public readonly Expr value;

            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }

        public class Logical : Expr
        {
            public readonly Expr left;
            public readonly Token operatorToken;
            public readonly Expr right;

            public Logical(Expr left, Token operatorToken, Expr right)
            {
                this.left = left;
                this.operatorToken = operatorToken;
                this.right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }

        public class Call : Expr
        {
            public readonly Expr callee;
            public readonly Token paren;
            public readonly List<Expr> arguments;

            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        public class Get : Expr
        {
            public readonly Expr obj;
            public readonly Token name;

            public Get(Expr obj, Token name)
            {
                this.obj = obj;
                this.name = name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }

        public class Set : Expr
        {
            public readonly Expr obj;
            public readonly Token name;
            public readonly Expr value;

            public Set(Expr obj, Token name, Expr value)
            {
                this.obj = obj;
                this.name = name;
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSetExpr(this);
            }
        }

        public class This : Expr
        {
            public readonly Token keyword;

            public This(Token keyword)
            {
                this.keyword = keyword;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitThisExpr(this);
            }
        }

        public class Super : Expr
        {
            public readonly Token keyword;
            public readonly Token method;

            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSuperExpr(this);
            }
        }
    }
}