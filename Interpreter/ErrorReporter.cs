using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class ErrorReporter
    {
        List<Error> errors = new List<Error>();

        public bool HadError { get => errors.Count > 0; }

        public void Report(TextSpan span, string message)
        {
            errors.Add(new Error(span, message));
        }

        public void Display(TextWriter writer)
        {
            foreach (Error error in errors)
            {
                writer.WriteLine(error.ToString());
            }
        }
    }

    internal class Error
    {
        public TextSpan Span { get; private set; }
        public string Message { get; private set; }

        public Error(TextSpan span, string message)
        {
            Span = span;
            Message = message;
        }

        public override string ToString()
        {
            return $"[start: {Span.Start}, end: {Span.End}] Error: {Message}";
        }
    }
}
