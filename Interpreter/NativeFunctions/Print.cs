namespace Interpreter.NativeFunctions
{
    internal class Print : ICallable
    {
        public int Arity() => 1;

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Console.Write(arguments[0]);
            return null;
        }

        public override string? ToString() => "<native fn>";
    }
}
