﻿namespace Interpreter
{
    internal class Parser
    {
        private const int MAX_ARGUMENTS = 255;

        private readonly ErrorReporter errorReporter;
        private readonly List<Token> tokens;
        private int current = 0;

        private int loopCount = 0;

        public Parser(List<Token> tokens, ErrorReporter errorReporter)
        {
            this.tokens = tokens;
            this.errorReporter = errorReporter;
        }

        public List<Statement> Parse()
        {
            List<Statement> statements = new List<Statement>();
            while (!IsAtEnd())
            {
                Statement? s = Declaration();
                if (s != null)
                {
                    statements.Add(s);
                }
            }

            return statements;
        }

        #region Statement Grammar
        private Statement? Declaration()
        {
            try
            {
                if (Match(TokenType.VarKeyword))
                {
                    return VariableDeclaration();
                }

                if (Match(TokenType.FnKeyword))
                {
                    return new Statement.ExpressionStatement(FunctionDeclaration("function"));
                }

                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }
        private Statement.VariableDeclaration VariableDeclaration()
        {
            Token varToken = Previous();
            Token name = Consume(TokenType.Identifier, "Expected a variable name.");

            Expression? initializer = null;
            if (Match(TokenType.Equal))
            {
                initializer = Expression();
            }

            Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");

            return new Statement.VariableDeclaration(varToken, name, initializer);
        }
        private Statement Statement()
        {
            if (Match(TokenType.IfKeyword))
            {
                return IfStatement();
            }

            if (Match(TokenType.WhileKeyword))
            {
                return WhileStatement();
            }

            if (Match(TokenType.ForKeyword))
            {
                return ForStatement();
            }

            if (Match(TokenType.BreakKeyword))
            {
                return BreakStatement();
            }

            if (Match(TokenType.ReturnKeyword))
            {
                return ReturnStatement();
            }

            if (Match(TokenType.LeftBrace))
            {
                return Block();
            }

            return ExpressionStatement();
        }
        private Statement.ExpressionStatement ExpressionStatement()
        {
            Expression expression = Expression();
            Consume(TokenType.Semicolon, "Expected ';' after expression.");
            return new Statement.ExpressionStatement(expression);
        }
        private Statement.Block Block()
        {
            List<Statement> statements = new List<Statement>();
            Token opening = Previous();

            while (!Check(TokenType.RightBrace) && !IsAtEnd())
            {
                Statement? statement = Declaration();
                if (statement != null)
                {
                    statements.Add(statement);
                }
            }

            Token closing = Consume(TokenType.RightBrace, "Expected '}' after block.");

            return new Statement.Block(opening, statements, closing);
        }
        private Statement.If IfStatement()
        {
            Token keyword = Previous();
            Consume(TokenType.LeftParenthesis, "Expected '(' after 'if'.");
            Expression condition = Expression();
            Consume(TokenType.RightParenthesis, "Expected ')' after condition.");
            Statement thenBranch = Statement();
            Statement? elseBranch = null;
            if (Match(TokenType.ElseKeyword))
            {
                elseBranch = Statement();
            }

            return new Statement.If(keyword, condition, thenBranch, elseBranch);
        }
        private Statement.While WhileStatement()
        {
            Token keyword = Previous();
            Consume(TokenType.LeftParenthesis, "Expected '(' after 'while'.");
            Expression condition = Expression();
            Consume(TokenType.RightParenthesis, "Expected ')' after condition.");
            loopCount++;
            Statement body = Statement();
            loopCount--;

            return new Statement.While(keyword, condition, body);
        }
        private Statement ForStatement()
        {
            Token keyword = Previous();
            Consume(TokenType.LeftParenthesis, "Expected '(' after 'for'.");

            Statement? initializer = null;
            if (Match(TokenType.Semicolon))
            {
                initializer = null;
            }
            else if (Match(TokenType.VarKeyword))
            {
                initializer = VariableDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            Expression? condition = null;
            if (!Check(TokenType.Semicolon))
            {
                condition = Expression();
            }

            Consume(TokenType.Semicolon, "Expected ';' after loop condition.");

            Expression? increment = null;
            if (!Check(TokenType.RightParenthesis))
            {
                increment = Expression();
            }

            Consume(TokenType.RightParenthesis, "Expected ')' after for clauses.");

            loopCount++;
            Statement body = Statement();
            loopCount--;

            if (increment != null)
            {
                body = new Statement.Block(null, new List<Statement>([body, new Statement.ExpressionStatement(increment)]), null);
            }

            if (condition == null)
            {
                condition = new Expression.Literal(new Token(TokenType.TrueKeyword, "true", keyword.Span, true));
            }

            body = new Statement.While(keyword, condition, body);

            if (initializer != null)
            {
                body = new Statement.Block(null, new List<Statement>([initializer, body]), null);
            }

            return body;
        }
        private Statement.Return ReturnStatement()
        {
            Token keyword = Previous();
            Expression? expression = null;
            if (!Check(TokenType.Semicolon))
            {
                expression = Expression();
            }
            Consume(TokenType.Semicolon, "Expected ';' after return value.");
            return new Statement.Return(keyword, expression);
        }
        private Statement.Break BreakStatement()
        {
            Token keyword = Previous();
            if (loopCount == 0)
            {
                Error(keyword, "Can't break outside of a loop.");
            }

            Consume(TokenType.Semicolon, "Expected ';' after value.");
            return new Statement.Break(keyword);
        }
        #endregion

        #region Expression Grammar
        private Expression Expression()
        {
            return Assignment();
        }
        private Expression Assignment()
        {
            if (Match(TokenType.FnKeyword))
            {
                return FunctionDeclaration();
            }

            Expression expression = Or();

            if (Match(TokenType.Equal))
            {
                Token equal = Previous();
                Expression value = Assignment();

                if (expression is Expression.Variable varExpr)
                {
                    Token name = varExpr.Name;
                    return new Expression.Assignment(name, equal, value);
                }

                Error(equal, "Invalid assignment target.");
            }

            return expression;
        }
        private Expression.Function FunctionDeclaration(string kind = "function")
        {
            Token keyword = Previous();
            Token? name = null;
            if (Check(TokenType.Identifier))
            {
                name = Consume(TokenType.Identifier, $"Expected {kind} name.");
            }
            Consume(TokenType.LeftParenthesis, $"Expected '(' after {(name != null ? kind : "lambda")} declaration.");
            List<Token> parameters = new List<Token>();
            if (!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (parameters.Count >= MAX_ARGUMENTS)
                    {
                        Error(Peek(), $"Can't have more than {MAX_ARGUMENTS} parameters.");
                    }
                    parameters.Add(Consume(TokenType.Identifier, "Expected parameter name."));
                } while (Match(TokenType.Comma));
            }
            Consume(TokenType.RightParenthesis, "Expected ')' after parameters.");
            Consume(TokenType.LeftBrace, $"Expected '{{' before {kind} body.");
            Statement.Block block = Block();

            return new Expression.Function(keyword, name, parameters, block);

        }
        private Expression Or()
        {
            Expression left = And();

            while (Match(TokenType.OrKeyword))
            {
                Token op = Previous();
                Expression right = And();
                left = new Expression.Logical(left, op, right);
            }

            return left;
        }
        private Expression And()
        {
            Expression left = Equality();

            while (Match(TokenType.AndKeyword))
            {
                Token op = Previous();
                Expression right = Equality();
                left = new Expression.Logical(left, op, right);
            }

            return left;
        }
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
        private Expression Unary()
        {
            if (Match(TokenType.Bang, TokenType.Minus))
            {
                Token operatorToken = Previous();
                Expression right = Unary();
                return new Expression.Unary(operatorToken, right);
            }

            return Call();
        }
        private Expression Call()
        {
            Expression expression = Primary();

            while (true)
            {
                if (Match(TokenType.LeftParenthesis))
                {
                    expression = FinishCall(expression);
                }
                else
                {
                    break;
                }
            }

            return expression;
        }
        private Expression FinishCall(Expression callee)
        {
            Token leftParenthesis = Previous();
            List<Expression> arguments = new List<Expression>();
            if (!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (arguments.Count >= MAX_ARGUMENTS)
                    {
                        Error(Peek(), $"Can't have more than {MAX_ARGUMENTS} arguments.");
                    }
                    arguments.Add(Expression());
                } while (Match(TokenType.Comma));
            }

            Token rightParenthesis = Consume(TokenType.RightParenthesis, "Expected ')' after arguments.");

            return new Expression.Call(callee, leftParenthesis, arguments, rightParenthesis);
        }
        private Expression Primary()
        {
            // Literals
            if (Match(
                TokenType.StringLiteral,
                TokenType.NumberLiteral,
                TokenType.FalseKeyword,
                TokenType.TrueKeyword,
                TokenType.NilKeyword
                ))
            {
                return new Expression.Literal(Previous());
            }

            //Variable expression
            if (Match(TokenType.Identifier))
            {
                return new Expression.Variable(Previous());
            }

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
            {
                return Advance();
            }

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
        private ParseError Error(Token token, string message)
        {
            errorReporter.Report(token.Span, message, ErrorType.Compiler);
            return new ParseError();
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
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case TokenType.ClassKeyword:
                    case TokenType.FnKeyword:
                    case TokenType.IfKeyword:
                    case TokenType.ForKeyword:
                    case TokenType.WhileKeyword:
                    case TokenType.VarKeyword:
                    case TokenType.ReturnKeyword:
                        return;
                }

                Advance();
            }
        }

        internal class ParseError : Exception { }
        #endregion
    }


}
