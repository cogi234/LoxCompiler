namespace Interpreter.NativeFunctions
{
    internal class Print : ICallable
    {
        public int Arity() => 1;

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Console.Write(Stringify(arguments[0]));
            return null;
        }

        private string Stringify(object? value)
        {
            if (value == null)
            {
                return "nil";
            }

            return value.ToString() ?? "";
        }

        public override string? ToString() => "<native fn>";
    }
}
