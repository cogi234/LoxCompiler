using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Interpreter.Expression;

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
            T visit(ExpressionStatement statement);
            T visit(VariableDeclaration statement);
            T visit(Block statement);
            T visit(If statement);
            T visit(While statement);
            T visit(Print statement);
            T visit(Break statement);
        }
        public abstract T accept<T>(IVisitor<T> visitor);
        #endregion

        #region Statement classes
        internal class ExpressionStatement : Statement
        {
            public Expression Expression { get; }

            public ExpressionStatement(Expression expression) : base(expression.Span)
            {
                Expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        internal class VariableDeclaration : Statement
        {
            public Token Name { get; }
            public Expression? Initializer { get; }

            public VariableDeclaration(Token varToken, Token name, Expression? initializer)
                : base(TextSpan.FromBounds(varToken.Span.Start, initializer == null ? name.Span.End : initializer.Span.End))
            {
                Name = name;
                Initializer = initializer;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
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

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        internal class If : Statement
        {
            public Expression Condition { get; }
            public Statement ThenBranch { get; }
            public Statement? ElseBranch { get; }

            public If(Token keyword, Expression condition, Statement thenBranch, Statement? elseBranch)
                : base(TextSpan.FromBounds(keyword.Span.Start, (elseBranch ?? thenBranch).Span.End))
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        internal class While : Statement
        {
            public Expression Condition { get; }
            public Statement Body { get; }

            public While(Token keyword, Expression condition, Statement body)
                :base(TextSpan.FromBounds(keyword.Span.Start, body.Span.End))
            {
                Condition = condition;
                Body = body;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        internal class Print : Statement
        {
            public Expression Expression { get; }

            public Print(Token keyword, Expression expression) : base(TextSpan.FromBounds(keyword.Span.Start, expression.Span.End))
            {
                Expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        internal class Break : Statement
        {
            public Break(Token keyword) : base(keyword.Span) { }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        #endregion
    }
}
