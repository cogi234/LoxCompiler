using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal class ErrorReporter
    {
        static List<Error> errors = new List<Error>();

        public static void Report(TextSpan span, string message)
        {
            errors.Add(new Error(span, message));
        }

        public static void Display(TextWriter writer)
        {
            foreach (Error error in errors)
            {

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
    }
}
