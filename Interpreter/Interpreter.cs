using Interpreter.NativeFunctions;

namespace Interpreter
{
    internal class Interpreter : Expression.IVisitor<object?>, Statement.IVisitor<object?>/*Can't put void here in C#*/
    {
        private ErrorReporter errorReporter = new ErrorReporter();

        private readonly Environment globals = new Environment();
        private Environment environment;

        public Interpreter()
        {
            environment = globals;
            globals.Define("clock", new Clock());
            globals.Define("print", new Print());
        }

        public void Interpret(List<Statement> statements, ErrorReporter errorReporter)
        {
            this.errorReporter = errorReporter;
            try
            {
                foreach (Statement statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError error)
            {
                errorReporter.Report(error.token.Span, error.Message, ErrorType.Runtime);
            }
        }

        private void Execute(Statement statement)
        {
            statement.Accept(this);
        }
        private void ExecuteBlock(List<Statement> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Statement statement in statements)
                    Execute(statement);
            }
            finally
            {
                this.environment = previous;
            }
        }

        #region Statement Visitor
        public object? Visit(Statement.VariableDeclaration statement)
        {
            object? value = null;
            if (statement.Initializer != null)
                value = Evaluate(statement.Initializer);

            environment.Define(statement.Name.Lexeme, value);

            return null;
        }
        public object? Visit(Statement.ExpressionStatement statement)
        {
            Evaluate(statement.Expression);
            return null;
        }
        public object? Visit(Statement.Block statement)
        {
            ExecuteBlock(statement.Statements, new Environment(environment));
            return null;
        }
        public object? Visit(Statement.If statement)
        {
            if (GetBooleanValue(Evaluate(statement.Condition)))
                Execute(statement.ThenBranch);
            else if (statement.ElseBranch != null)
                Execute(statement.ElseBranch);
            return null;
        }
        public object? Visit(Statement.While statement)
        {
            try
            {
                while (GetBooleanValue(Evaluate(statement.Condition)))
                {
                    Execute(statement.Body);
                }
            }
            catch (BreakException)
            {
                return null;
            }
            return null;
        }
        public object? Visit(Statement.Break statement)
        {
            throw new BreakException();
        }
        #endregion

        #region Expression Visitor
        public object? Visit(Expression.Assignment expression)
        {
            object? value = Evaluate(expression.Value);
            environment.Assign(expression.Name, value);
            return value;
        }
        public object? Visit(Expression.Logical expression)
        {
            object? left = Evaluate(expression.Left);

            if (expression.Operator.Type == TokenType.OrKeyword)
            {
                if (GetBooleanValue(left))
                    return left;
            }
            else if (expression.Operator.Type == TokenType.AndKeyword)
            {
                if (!GetBooleanValue(left))
                    return left;
            }

            return Evaluate(expression.Right);
        }
        public object? Visit(Expression.Binary expression)
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
        public object? Visit(Expression.Unary expression)
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
        public object? Visit(Expression.Call expression)
        {
            object? callee = Evaluate(expression.Callee);

            List<object?> arguments = new List<object?>();
            foreach (Expression argument in expression.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ICallable))
            {
                throw new RuntimeError(expression.LeftParenthesis, "Can only call functions");
            }

            ICallable function = (ICallable)callee;
            if (arguments.Count != function.Arity())
            {
                throw new RuntimeError(expression.LeftParenthesis, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }
        public object? Visit(Expression.Grouping expression)
        {
            return Evaluate(expression.Expression);
        }
        public object? Visit(Expression.Literal expression)
        {
            return expression.LiteralToken.Literal;
        }
        public object? Visit(Expression.Variable expression)
        {
            return environment.Get(expression.Name);
        }
        #endregion

        #region Helpers
        private object? Evaluate(Expression expression)
        {
            return expression.Accept(this);
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
            throw new RuntimeError(operatorToken, "Operands must be numbers.");
        }
        internal class RuntimeError : Exception
        {
            public readonly Token token;

            public RuntimeError(Token token, string message) : base(message)
            {
                this.token = token;
            }
        }
        internal class BreakException : Exception { }
        #endregion
    }
}
