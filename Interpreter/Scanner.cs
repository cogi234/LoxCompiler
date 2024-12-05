using System.Text;

namespace Interpreter
{
    internal class Scanner
    {
        #region Static Members
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "true", TokenType.TrueKeyword },
            { "false", TokenType.FalseKeyword },

            { "and", TokenType.AndKeyword },
            { "or", TokenType.OrKeyword },

            { "if", TokenType.IfKeyword },
            { "else", TokenType.ElseKeyword },
            { "while", TokenType.WhileKeyword },
            { "for", TokenType.ForKeyword },
            { "break", TokenType.BreakKeyword },

            { "class", TokenType.ClassKeyword },
            { "new", TokenType.NewKeyword },
            { "this", TokenType.ThisKeyword },

            { "function", TokenType.FunctionKeyword },
            { "print", TokenType.PrintKeyword },
            { "super", TokenType.SuperKeyword },
            { "return", TokenType.ReturnKeyword },

            { "var", TokenType.VarKeyword },
            { "nil", TokenType.NilKeyword },
        };
        #endregion

        #region Members
        private readonly ErrorReporter errorReporter;
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();

        private int start = 0, current = 0;

        public Scanner(string source, ErrorReporter errorReporter)
        {
            this.source = source;
            this.errorReporter = errorReporter;
        }
        #endregion

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme
                start = current;
                ScanToken();
            }
            // We insert and end of file at the end of the token list
            tokens.Add(new Token(TokenType.EndOfFile, "\0", new TextSpan(current, 0), null));
            return tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                // Single characters
                case '(': AddToken(TokenType.LeftParenthesis); break;
                case ')': AddToken(TokenType.RightParenthesis); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case '*': AddToken(TokenType.Star); break;
                case '%': AddToken(TokenType.Percent); break;
                // Double characters
                case '!':
                    AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LesserEqual : TokenType.Lesser);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A line comment.
                        while (Peek() != '\n' && !IsAtEnd())
                            Advance();
                    }
                    else if (Match('*'))
                    {
                        // A multi-line comment.
                        while (!MatchTwo('*', '/') && !IsAtEnd())
                            Advance();
                    }
                    else
                        AddToken(TokenType.Slash);
                    break;
                // Whitespace
                case ' ':
                case '\r':
                case '\t':
                case '\n':
                    break;
                // Variable length
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                        Number();
                    else if (IsAlpha(c))
                        Identifier();
                    else
                        errorReporter.Report(new TextSpan(current, 1), "Unexpected character.", ErrorType.Compiler);
                    break;
            }
        }

        private void String()
        {
            StringBuilder sb = new StringBuilder();
            // We advance until we find the next quote(unless we escape it), or the end of the source
            while (true)
            {
                //Escape sequences
                if (Match('\\'))
                {
                    int escapeStart = current - 1;
                    switch (Advance())
                    {
                        case '\\': sb.Append('\\'); break;
                        case '"': sb.Append('\"'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case '0': sb.Append('\0'); break;
                        default:
                            errorReporter.Report(TextSpan.FromBounds(escapeStart, current), "Unrecognized escape sequence.", ErrorType.Compiler);
                            break;
                    }
                }
                else if (Peek() == '"' || IsAtEnd())
                    break;
                else
                    sb.Append(Advance());
            }

            if (IsAtEnd())
            {
                TextSpan errorSpan = TextSpan.FromBounds(start, current);
                errorReporter.Report(errorSpan, "Unterminated string.", ErrorType.Compiler);
                return;
            }

            Advance();// Consume the closing ".

            AddToken(TokenType.StringLiteral, sb.ToString());
        }

        private void Number()
        {
            // Whole part
            while (IsDigit(Peek()))
                Advance();

            // Fractionnal part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();// Consume the "."
                while (IsDigit(Peek()))
                    Advance();
            }

            TextSpan span = TextSpan.FromBounds(start, current);

            double value = double.Parse(source.Substring(span.Start, span.Length));
            AddToken(TokenType.NumberLiteral, value);
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
                Advance();

            TextSpan span = TextSpan.FromBounds(start, current);
            string text = source.Substring(span.Start, span.Length);
            if (keywords.ContainsKey(text))
                AddToken(keywords[text]);
            else
                AddToken(TokenType.Identifier);
        }

        #region Helper Methods
        private char Advance() => source[current++];
        private char Peek() => IsAtEnd() ? '\0' : source[current];
        private char PeekNext() => current + 1 >= source.Length ? '\0' : source[current + 1];
        private bool Match(char expected)
        {
            if (Peek() != expected)
                return false;

            current++;
            return true;
        }
        private bool MatchTwo(char first, char second)
        {
            if (Peek() != first || PeekNext() != second)
                return false;

            current += 2;
            return true;
        }

        private bool IsAtEnd() => current >= source.Length;
        private bool IsDigit(char c) => c >= '0' && c <= '9';
        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') ||
                                        (c >= 'A' && c <= 'Z') ||
                                        c == '_';
        private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        private void AddToken(TokenType type, object? literal = null)
        {
            TextSpan span = TextSpan.FromBounds(start, current);
            string text = source.Substring(span.Start, span.Length);
            tokens.Add(new Token(type, text, span, literal));
        }
        #endregion
    }
}
