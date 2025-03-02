namespace Interpreter
{
    internal interface ICallable
    {
        int Arity();
        object? Call(Interpreter interpreter, List<object?> arguments);
    }
}
