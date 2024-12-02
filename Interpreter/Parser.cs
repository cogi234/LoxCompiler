﻿namespace Interpreter
{
    internal class Parser
    {
        private readonly ErrorReporter errorReporter;
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens, ErrorReporter errorReporter)
        {
            this.tokens = tokens;
            this.errorReporter = errorReporter;
        }

        public Expression? Parse()
        {
            try
            {
                return Expression();
            } catch(ParserError error)
            {
                return null;
            }
        }

        #region Expression Grammar
        /// <summary>
        /// expression -> equality
        /// </summary>
        private Expression Expression()
        {
            return Equality();
        }

        /// <summary>
        /// equality -> comparison ( ( "!=" | "==" ) comparison )* ;
        /// </summary>
        private Expression Equality()
        {
            if (Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                Error(Previous(), "Missing left side of the comparison.");
                return Comparison();
            }

            Expression expression = Comparison();

            while (Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                Token operatorToken = Previous();
                Expression right = Comparison();
                expression = new Expression.Binary(expression, operatorToken, right);
            }

            return expression;
        }

        /// <summary>
        /// comparison -> term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
        /// </summary>
        private Expression Comparison()
        {
            if (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Lesser, TokenType.LesserEqual))
            {
                Error(Previous(), "Missing left side of the comparison.");
                return Term();
            }

            Expression expression = Term();

            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Lesser, TokenType.LesserEqual))
            {
                Token operatorToken = Previous();
                Expression right = Term();
                expression = new Expression.Binary(expression, operatorToken, right);
            }

            return expression;
        }

        /// <summary>
        /// term -> factor ( ( "-" | "+" ) factor )* ;
        /// </summary>
        private Expression Term()
        {
            if (Match(TokenType.Plus))
            {
                Error(Previous(), "Missing left side of the operation.");
                return Factor();
            }

            Expression expression = Factor();

            while (Match(TokenType.Minus, TokenType.Plus))
            {
                Token operatorToken = Previous();
                Expression right = Factor();
                expression = new Expression.Binary(expression, operatorToken, right);
            }

            return expression;
        }

        /// <summary>
        /// factor -> unary ( ( "/" | "*" | "%" ) unary )* ;
        /// </summary>
        private Expression Factor()
        {
            if (Match(TokenType.Slash, TokenType.Star, TokenType.Percent))
            {
                Error(Previous(), "Missing left side of the operation.");
                return Unary();
            }

            Expression expression = Unary();

            while (Match(TokenType.Slash, TokenType.Star, TokenType.Percent))
            {
                Token operatorToken = Previous();
                Expression right = Unary();
                expression = new Expression.Binary(expression, operatorToken, right);
            }

            return expression;
        }

        /// <summary>
        /// unary -> ( "!" | "-" ) unary 
        ///         | primary ;
        /// </summary>
        private Expression Unary()
        {
            if (Match(TokenType.Bang, TokenType.Minus))
            {
                Token operatorToken = Previous();
                Expression right = Unary();
                return new Expression.Unary(operatorToken, right);
            }

            return Primary();
        }

        private Expression Primary()
        {
            // Literals
            if (Match(
                TokenType.StringLiteral,
                TokenType.IntegerLiteral,
                TokenType.FloatLiteral,
                TokenType.FalseKeyword,
                TokenType.TrueKeyword,
                TokenType.NilKeyword
                )) 
                return new Expression.Literal(Previous());

            // Grouping
            if (Match(TokenType.LeftParenthesis))
            {
                Token leftParenthesis = Previous();
                Expression expression = Expression();
                Token rightParenthesis = Consume(TokenType.RightParenthesis, "Expected ')' after expression.");
                return new Expression.Grouping(leftParenthesis, expression, rightParenthesis);
            }

            throw Error(Peek(), "Expected expression.");
        }
        #endregion

        #region Helpers
        private Token Advance() => IsAtEnd() ? tokens[current] : tokens[current++];
        private Token Peek() => tokens[current];
        private Token Previous() => tokens[current - 1];
        private Token Consume(TokenType type, string errorMessage)
        {
            if (Check(type))
                return Advance();

            throw Error(Peek(), errorMessage);
        }
        private bool Check(TokenType type) => !IsAtEnd() && Peek().Type == type;
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    current++;
                    return true;
                }
            }

            return false;
        }

        private bool IsAtEnd() => Peek().Type == TokenType.EndOfFile;
        #endregion

        #region Error handling
        private ParserError Error(Token token, string message)
        {
            errorReporter.Report(token.Span, message);
            return new ParserError();
        }

        /// <summary>
        /// Try to advance to a statement boundary
        /// </summary>
        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon)
                    return;

                switch (Peek().Type)
                {
                    case TokenType.ClassKeyword:
                    case TokenType.FunctionKeyword:
                    case TokenType.IfKeyword:
                    case TokenType.ForKeyword:
                    case TokenType.WhileKeyword:
                    case TokenType.VarKeyword:
                    case TokenType.IntKeyword:
                    case TokenType.FloatKeyword:
                    case TokenType.StringKeyword:
                    case TokenType.ReturnKeyword:
                    case TokenType.PrintKeyword:
                        return;
                }

                Advance();
            }
        }

        internal class ParserError : Exception { }
        #endregion
    }


}
