namespace Interpreter
{
    internal class Environment
    {
        private readonly Dictionary<string, object?> values = new Dictionary<string, object?>();
        private readonly Environment? enclosing;

        public Environment()
        {
            enclosing = null;
        }
        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public void Define(string name, object? value)
        {
            values[name] = value;
        }

        public void Assign(Token name, object? value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new Interpreter.RuntimeError(name, $"Undefined variable '${name.Lexeme}'.");
        }

        public object? Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
                return values[name.Lexeme];

            if (enclosing != null)
                return enclosing.Get(name);

            throw new Interpreter.RuntimeError(name, $"Undefined variable '${name.Lexeme}'.");
        }
    }
}
