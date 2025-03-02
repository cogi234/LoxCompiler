namespace Interpreter
{
    internal abstract class Statement
    {
        public TextSpan Span { get; }
        public Statement(TextSpan span)
        {
            Span = span;
        }

        #region Visitor Pattern
        internal interface IVisitor<T>
        {
            T Visit(ExpressionStatement statement);
            T Visit(VariableDeclaration statement);
            T Visit(Function statement);
            T Visit(Block statement);
            T Visit(If statement);
            T Visit(While statement);
            T Visit(Return statement);
            T Visit(Break statement);
        }
        public abstract T Accept<T>(IVisitor<T> visitor);
        #endregion

        #region Statement classes
        internal class ExpressionStatement : Statement
        {
            public Expression Expression { get; }

            public ExpressionStatement(Expression expression) : base(expression.Span)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class VariableDeclaration : Statement
        {
            public Token Keyword { get; }
            public Token Name { get; }
            public Expression? Initializer { get; }

            public VariableDeclaration(Token keyword, Token name, Expression? initializer)
                : base(TextSpan.FromBounds(keyword.Span.Start, initializer == null ? name.Span.End : initializer.Span.End))
            {
                Keyword = keyword;
                Name = name;
                Initializer = initializer;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Function : Statement
        {
            public Token Name { get; }
            public List<Token> Parameters { get; }
            public Block Body { get; }

            public Function(Token name, List<Token> parameters, Block body)
                : base(TextSpan.FromBounds(name.Span.Start, body.Span.End))
            {
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Block : Statement
        {
            public List<Statement> Statements { get; }

            public Block(Token? opening, List<Statement> statements, Token? closing)
                : base(TextSpan.FromBounds(opening == null ? statements[0].Span.Start : opening.Span.Start,
                                           closing == null ? statements[statements.Count - 1].Span.End : closing.Span.End))
            {
                Statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class If : Statement
        {
            public Token Keyword { get; }
            public Expression Condition { get; }
            public Statement ThenBranch { get; }
            public Statement? ElseBranch { get; }

            public If(Token keyword, Expression condition, Statement thenBranch, Statement? elseBranch)
                : base(TextSpan.FromBounds(keyword.Span.Start, (elseBranch ?? thenBranch).Span.End))
            {
                Keyword = keyword;
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class While : Statement
        {
            public Token Keyword { get; }
            public Expression Condition { get; }
            public Statement Body { get; }

            public While(Token keyword, Expression condition, Statement body)
                : base(TextSpan.FromBounds(keyword.Span.Start, body.Span.End))
            {
                Keyword = keyword;
                Condition = condition;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Return : Statement
        {
            public Token Keyword { get; }
            public Expression? Expression { get; }

            public Return(Token keyword, Expression? expression) : base(expression == null ? keyword.Span
                                                                                           : TextSpan.FromBounds(keyword.Span.Start, expression.Span.End))
            {
                Keyword = keyword;
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        internal class Break : Statement
        {
            public Token Keyword { get; }

            public Break(Token keyword) : base(keyword.Span)
            {
                Keyword = keyword;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        #endregion
    }
}
