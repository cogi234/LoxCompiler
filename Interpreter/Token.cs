namespace Interpreter
{
    internal class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public TextSpan Span { get; }
        public object? Literal { get; }

        public Token(TokenType type, string lexeme, TextSpan span, object? literal)
        {
            Type = type;
            Lexeme = lexeme;
            Span = span;
            Literal = literal;
        }

        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }

    enum TokenType
    {
        // Single character tokens
        LeftParenthesis, RightParenthesis,
        LeftBrace, RightBrace,
        Comma, Dot, Semicolon,
        Minus, Plus, Slash, Star, Percent,

        // One/two character tokens
        Bang, BangEqual,
        Equal, EqualEqual,
        Greater, GreaterEqual,
        Lesser, LesserEqual,

        // Literals
        Identifier, StringLiteral, NumberLiteral,

        // Keywords
        TrueKeyword, FalseKeyword,
        AndKeyword, OrKeyword,
        IfKeyword, ElseKeyword,
        WhileKeyword, ForKeyword, BreakKeyword,
        ClassKeyword, NewKeyword, ThisKeyword,
        FnKeyword, SuperKeyword, ReturnKeyword,
        VarKeyword, NilKeyword,

        // Misc
        EndOfFile
    }
}
