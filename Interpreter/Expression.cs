using static Interpreter.Statement;

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
            T Visit(Assignment expression);
            T Visit(Function expression);
            T Visit(Logical expression);
            T Visit(Binary expression);
            T Visit(Unary expression);
            T Visit(Call expression);
            T Visit(Grouping expression);
            T Visit(Literal expression);
            T Visit(Variable expression);
        }
        public abstract T Accept<T>(IVisitor<T> visitor);
        #endregion

        #region Expression classes
        internal class Assignment : Expression
        {
            public Token Name { get; }
            public Token Operator { get; }
            public Expression Value { get; }

            public Assignment(Token name, Token operatorToken, Expression value) :
                base(TextSpan.FromBounds(name.Span.Start, value.Span.End))
            {
                Name = name;
                Operator = operatorToken;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Function : Expression
        {
            public Token Keyword { get; }
            public string Name => NameToken?.Lexeme ?? "lambda";
            public Token? NameToken { get; }
            public List<Token> Parameters { get; }
            public Block Body { get; }


            public Function(Token keyword, Token? nameToken, List<Token> parameters, Block body)
                : base(TextSpan.FromBounds(keyword.Span.Start, body.Span.End))
            {
                Keyword = keyword;
                NameToken = nameToken;
                Parameters = parameters;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
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

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Logical : Expression
        {
            public Expression Left { get; }
            public Token Operator { get; }
            public Expression Right { get; }

            public Logical(Expression left, Token operatorToken, Expression right) :
                base(TextSpan.FromBounds(left.Span.Start, right.Span.End))
            {
                Left = left;
                Operator = operatorToken;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
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

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Call : Expression
        {
            public Expression Callee { get; }
            public Token LeftParenthesis { get; }
            public List<Expression> Arguments { get; }
            public Token RightParenthesis { get; }

            public Call(Expression callee, Token leftParenthesis, List<Expression> arguments, Token rightParenthesis) :
                base(TextSpan.FromBounds(callee.Span.Start, rightParenthesis.Span.End))
            {
                Callee = callee;
                LeftParenthesis = leftParenthesis;
                Arguments = arguments;
                RightParenthesis = rightParenthesis;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Grouping : Expression
        {
            public Token LeftParenthesis { get; }
            public Expression Expression { get; }
            public Token RightParenthesis { get; }

            public Grouping(Token leftParenthesis, Expression expression, Token rightParenthesis) :
                base(TextSpan.FromBounds(leftParenthesis.Span.Start, rightParenthesis.Span.End))
            {
                LeftParenthesis = leftParenthesis;
                Expression = expression;
                RightParenthesis = rightParenthesis;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Literal : Expression
        {
            public Token LiteralToken { get; }

            public Literal(Token literalToken) : base(literalToken.Span)
            {
                LiteralToken = literalToken;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Variable : Expression
        {
            public Token Name { get; }

            public Variable(Token name) : base(name.Span)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        #endregion
    }
}
