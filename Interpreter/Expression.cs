namespace Interpreter
{
    internal abstract class Expression
    {
        public TextSpan Span { get; }
        public Expression(TextSpan span)
        {
            Span = span;
        }

        #region Visitor Pattern
        internal interface IVisitor<T>
        {
            T visit(Binary expression);
            T visit(Unary expression);
            T visit(Grouping expression);
            T visit(Literal expression);
        }
        public abstract T accept<T>(IVisitor<T> visitor);
        #endregion

        #region Expression classes
        internal class Binary : Expression
        {
            public Expression Left { get; }
            public Token Operator { get; }
            public Expression Right { get; }

            public Binary(Expression left, Token operatorToken, Expression right) :
                base(TextSpan.FromBounds(left.Span.Start, right.Span.End))
            {
                Left = left;
                Operator = operatorToken;
                Right = right;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        internal class Unary : Expression
        {
            public Token Operator { get; }
            public Expression Expression { get; }

            public Unary(Token operatorToken, Expression expression) :
                base(TextSpan.FromBounds(operatorToken.Span.Start, expression.Span.End))
            {
                Operator = operatorToken;
                Expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        internal class Grouping : Expression
        {
            public Expression Expression { get; }

            public Grouping(Token leftParenthesis, Expression expression, Token rightParenthesis) :
                base(TextSpan.FromBounds(leftParenthesis.Span.Start, rightParenthesis.Span.End))
            {
                Expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        internal class Literal : Expression
        {
            public Token LiteralToken { get; }

            public Literal(Token literalToken) : base(literalToken.Span)
            {
                LiteralToken = literalToken;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        #endregion
    }
}
