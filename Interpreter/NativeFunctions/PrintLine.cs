namespace Interpreter.NativeFunctions
{
    internal class PrintLine : ICallable
    {
        public int Arity() => 1;

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Console.WriteLine(Stringify(arguments[0]));
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
