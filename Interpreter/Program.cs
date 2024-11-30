namespace Interpreter
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length > 1) {
                Console.WriteLine("Usage: lox [script]");
                return 64;
            }


            if (args.Length == 1)
            {
                ErrorReporter errorReporter = new ErrorReporter();
                RunFile(args[0], errorReporter);
                if (errorReporter.HadError)
                    return 65;
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

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }

            errorReporter.Display(Console.Error);
        }
    }
}
