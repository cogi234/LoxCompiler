namespace Interpreter.NativeFunctions
{
    internal class Clock : ICallable
    {
        public int Arity() => 0;

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d;
        }

        public override string? ToString() => "<native fn>";
    }
}
