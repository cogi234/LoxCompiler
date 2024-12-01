using System.Text;

namespace Interpreter
{
    internal class AstPrinter : Expression.IVisitor<string>
    {
        public string print(Expression expression)
        {
            return expression.accept(this);
        }

        public string visit(Expression.Binary expression)
            => Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);

        public string visit(Expression.Unary expression)
            => Parenthesize(expression.Operator.Lexeme, expression.Expression);

        public string visit(Expression.Grouping expression)
            => Parenthesize("group", expression.Expression);

        public string visit(Expression.Literal expression)
            => expression.LiteralToken.Literal == null ? "nil" : expression.LiteralToken.Literal.ToString() ?? "";

        #region Helpers
        private string Parenthesize(string name, params Expression[] expressions)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(").Append(name);
            foreach (Expression expression in expressions)
            {
                sb.Append(" ").Append(expression.accept(this));
            }
            sb.Append(")");

            return sb.ToString();
        }
        #endregion
    }
}
