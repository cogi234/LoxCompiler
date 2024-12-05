using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class Environment
    {
        private readonly Dictionary<string, object?> values = new Dictionary<string, object?>();

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

            throw new Interpreter.RuntimeError(name, $"Undefined variable '${name.Lexeme}'.");
        }

        public object? Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
                return values[name.Lexeme];
            throw new Interpreter.RuntimeError(name, $"Undefined variable '${name.Lexeme}'.");
        }
    }
}
