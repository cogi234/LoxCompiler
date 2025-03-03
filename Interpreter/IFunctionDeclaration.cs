namespace Interpreter
{
    internal interface IFunctionDeclaration
    {
        List<Token> Parameters { get; }
        Statement.Block Body { get; }
        string Name { get; }
    }
}
