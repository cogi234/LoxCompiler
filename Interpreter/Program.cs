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
                RunFile(args[0]);
                return 0;
            }

            RunPrompt();
            return 0;
        }

        static void RunFile(string path)
        {
            StreamReader reader = new StreamReader(path);
            Run(reader.ReadToEnd());
        }

        static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null || line == "exit")
                    break;
                Run(line);
            }
        }

        static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
}
