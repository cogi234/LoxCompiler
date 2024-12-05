using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class Interpreter : Expression.IVisitor<object?>, Statement.IVisitor<object?>/*Can't put void here in C#*/
    {
        private ErrorReporter errorReporter = new ErrorReporter();
        private Environment environment = new Environment();

        public void Interpret(List<Statement> statements, ErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
            try
            {
                foreach (Statement statement in statements)
                {
                    Execute(statement);
                }
            } catch (RuntimeError error)
            {
                errorReporter.Report(error.token.Span, error.Message, ErrorType.Runtime);
            }
        }

        private void Execute(Statement statement)
        {
            statement.accept(this);
        }
        private void ExecuteBlock(List<Statement> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Statement statement in statements)
                    Execute(statement);
            } finally
            {
                this.environment = previous;
            }
        }

        #region Statement Visitor
        public object? visit(Statement.VariableDeclaration statement)
        {
            object? value = null;
            if (statement.Initializer != null)
                value = Evaluate(statement.Initializer);

            environment.Define(statement.Name.Lexeme, value);

            return null;
        }
        public object? visit(Statement.ExpressionStatement statement)
        {
            Evaluate(statement.Expression);
            return null;
        }
        public object? visit(Statement.Block statement)
        {
            ExecuteBlock(statement.Statements, new Environment(environment));
            return null;
        }
        public object? visit(Statement.Print statement)
        {
            object? value = Evaluate(statement.Expression);
            Console.WriteLine(Stringify(value));
            return null;
        }
        #endregion

        #region Expression Visitor
        public object? visit(Expression.Assignment expression)
        {
            object? value = Evaluate(expression.Value);
            environment.Assign(expression.Name, value);
            return value;
        }
        public object? visit(Expression.Binary expression)
        {
            object? left = Evaluate(expression.Left);
            object? right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
                case TokenType.EqualEqual:
                    return IsEqual(left, right);
                case TokenType.Greater:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.Lesser:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LesserEqual:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left <= (double)right;
                case TokenType.Minus:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.Plus:
                    if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                        return (double)left + (double)right;
                    if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                        return (string)left + (string)right;
                    throw new RuntimeError(expression.Operator, "Operands must be two numbers or two strings.");
                case TokenType.Slash:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.Star:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left * (double)right;
            }

            return null;
        }
        public object? visit(Expression.Unary expression)
        {
            object? right = Evaluate(expression.Expression);

            switch (expression.Operator.Type)
            {
                case TokenType.Bang:
                    return !GetBooleanValue(right);
                case TokenType.Minus:
                    CheckNumberOperand(expression.Operator, right);
                    return -(double)right;
            }

            return null;
        }
        public object? visit(Expression.Grouping expression)
        {
            return Evaluate(expression.Expression);
        }
        public object? visit(Expression.Literal expression)
        {
            return expression.LiteralToken.Literal;
        }
        public object? visit(Expression.Variable expression)
        {
            return environment.Get(expression.Name);
        }
        #endregion

        #region Helpers
        private object? Evaluate(Expression expression)
        {
            return expression.accept(this);
        }
        /// <summary>
        /// Only nil and false are false. Everything else is true
        /// </summary>
        private bool GetBooleanValue(object? value)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(bool))
                return (bool)value;
            return true;
        }
        private bool IsEqual(object? a, object? b)
        {
            if (a == null)
                return b == null;
            return a.Equals(b);
        }
        private string Stringify(object? value)
        {
            if (value == null)
                return "nil";

            return value.ToString() ?? "";
        }
        #endregion

        #region Error handling
        private void CheckNumberOperand(Token operatorToken, object? operand)
        {
            if (operand != null && operand.GetType() == typeof(double))
                return;
            throw new RuntimeError(operatorToken, "Operand must be a number.");
        }
        private void CheckNumberOperands(Token operatorToken, object? left, object? right)
        {
            if (left != null && left.GetType() == typeof(double)
                && right != null && right.GetType() == typeof(double))
                return;
            throw new RuntimeError(operatorToken, "Operand musts be numbers.");
        }
        internal class RuntimeError : Exception {
            public readonly Token token;

            public RuntimeError(Token token, string message) : base(message)
            {
                this.token = token;
            }
        }
        #endregion
    }
}
