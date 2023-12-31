namespace CSharp
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
            public Expr Expression { get; }

            public Grouping(Expr expression)
            {
                Expression = expression;
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
            public Expr Right { get; }

            public Unary(Token operatorToken, Expr right)
            {
                OperatorToken = operatorToken;
                Right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public class Variable : Expr
        {
            public readonly Token Name;

            public Variable(Token name)
            {
                Name = name;
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
            public readonly Expr Callee;
            public readonly Token Paren;
            public readonly List<Expr> Arguments;

            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

    }
}