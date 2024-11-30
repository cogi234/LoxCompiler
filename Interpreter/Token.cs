using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Identifier, StringLiteral, IntegerLiteral, FloatLiteral,

        // Keywords
        TrueKeyword, FalseKeyword,
        AndKeyword, OrKeyword,
        IfKeyword, ElseKeyword,
        WhileKeyword, ForKeyword, BreakKeyword,
        ClassKeyword, NewKeyword, ThisKeyword,
        FunctionKeyword, PrintKeyword, SuperKeyword,
        VarKeyword, StringKeyword, IntKeyword, FloatKeyword, NilKeyword,

        // Misc
        EndOfFile
    }
}
