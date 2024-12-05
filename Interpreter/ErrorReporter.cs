using System.Reflection.Metadata.Ecma335;

namespace Interpreter
{
    internal class ErrorReporter
    {
        List<Error> errors = new List<Error>();

        public bool HadCompilerError
        {
            get => errors.Any((Error error) =>
            {
                return error.Type == ErrorType.Compiler;
            });
        }
        public bool HadRuntimeError {
            get => errors.Any((Error error) =>
            {
                return error.Type == ErrorType.Runtime;
            });
        }

        public void Report(TextSpan span, string message, ErrorType errorType)
        {
            errors.Add(new Error(span, message, errorType));
        }

        public void Display(TextWriter writer, params ErrorType[] typesToDisplay)
        {
            //We only display the errors of the specified types
            foreach (Error error in errors.Where((Error error) => typesToDisplay.Contains(error.Type)))
            {
                writer.WriteLine(error.ToString());
            }
        }
    }

    internal class Error
    {
        public TextSpan Span { get; private set; }
        public string Message { get; private set; }
        public ErrorType Type { get; private set; }

        public Error(TextSpan span, string message, ErrorType type)
        {
            Span = span;
            Message = message;
            Type = type;
        }

        public override string ToString()
        {
            return $"[start: {Span.Start}, length: {Span.Length}] {Type} error: {Message}";
        }

    }
    internal enum ErrorType
    {
        Compiler,
        Runtime
    }
}
