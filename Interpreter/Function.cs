using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class Function : ICallable
    {
        private readonly Statement.Function declaration;

        public Function(Statement.Function declaration)
        {
            this.declaration = declaration;
        }

        public int Arity() => declaration.Parameters.Count;

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new Environment(interpreter.globals);

            for (int i = 0; i < declaration.Parameters.Count; i++)
            {
                Token? parameter = declaration.Parameters[i];
                environment.Define(parameter.Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body.Statements, environment);
            } catch (Interpreter.ReturnException ex)
            {
                return ex.Value;
            }
            return null;
        }

        public override string? ToString() => $"<fn {declaration.Name.Lexeme}>";
    }
}
