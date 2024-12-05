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
            T visit(VariableDeclaration statement);
            T visit(ExpressionStatement statement);
            T visit(Print statement);
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
        internal class Print : Statement
        {
            public Expression Expression { get; }

            public Print(Expression expression) : base(expression.Span)
            {
                Expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }
        #endregion
    }
}
