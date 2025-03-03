namespace Interpreter
{
    internal class Function : ICallable
    {
        private readonly IFunctionDeclaration declaration;
        private readonly Environment closure;

        public Function(IFunctionDeclaration declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }

        public int Arity() => declaration.Parameters.Count;

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new Environment(closure);

            for (int i = 0; i < declaration.Parameters.Count; i++)
            {
                Token? parameter = declaration.Parameters[i];
                environment.Define(parameter.Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body.Statements, environment);
            }
            catch (Interpreter.ReturnException ex)
            {
                return ex.Value;
            }
            return null;
        }

        public override string? ToString() => $"<fn {declaration.Name}>";
    }
}
