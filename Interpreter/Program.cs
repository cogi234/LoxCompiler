namespace Interpreter
{
    internal class Program
    {
        private static readonly Interpreter interpreter = new Interpreter();
        static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                return 64;
            }


            if (args.Length == 1)
            {
                ErrorReporter errorReporter = new ErrorReporter();
                RunFile(args[0], errorReporter);
                if (errorReporter.HadCompilerError)
                    return 65;
                else if (errorReporter.HadRuntimeError)
                    return 70;
                else
                    return 0;
            }

            RunPrompt();
            return 0;
        }

        static void RunFile(string path, ErrorReporter errorReporter)
        {
            StreamReader reader = new StreamReader(path);
            Run(reader.ReadToEnd(), errorReporter);
            Console.ReadLine();
        }

        static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null || line == "exit")
                    break;

                ErrorReporter errorReporter = new ErrorReporter();
                Run(line, errorReporter);
            }
        }

        static void Run(string source, ErrorReporter errorReporter)
        {
            Scanner scanner = new Scanner(source, errorReporter);
            List<Token> tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens, errorReporter);
            List<Statement> statements = parser.Parse();

            if (errorReporter.HadCompilerError)
            {
                errorReporter.Display(Console.Error, ErrorType.Compiler);
                return;
            }

            interpreter.Interpret(statements, errorReporter);
            if (errorReporter.HadRuntimeError)
                errorReporter.Display(Console.Error, ErrorType.Runtime);
        }
    }
}
