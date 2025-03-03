## Notation:
nonterminal -> "terminal" nonterminal LITERAL_TERMINAL ;
| : or
() : grouping
\* : repeat 0+ times
\+ : repeat 1+ times
? : optional
## Statements
```
program             -> statement\* EOF ;
declaration         -> variableDeclaration | functionDeclaration | statement ;
variableDeclaration -> "var" IDENTIFIER ( "=" expression )? ";" ;
functionDeclaration -> "fn" function ;
function            -> IDENTIFIER "(" parameters? ")" block ;
statement           -> expressionStatement | printStatement | breakStatement |                            ifStatement | whileStatement| forStatement | block |                               returnStatement ;
forStatement        -> "for" "(" ( variableDeclaration | expressionStatement | ";"                                   ) expression? ";" expression? ")" statement ;
whileStatement      -> "while" "(" expression ")" statement;
ifStatement         -> "if" "(" expression ")" statement ( "else" statement)? ;
expressionStatement -> expression ";" ;
returnStatement     -> "return" expression? ";" ;
breakStatement      -> "break" ";" ;

```
## Expressions
```
expression -> assignment ;
assigment  -> IDENTIFIER "=" assignment
              | logicalOr | lambda;
lambda     -> "fn" "(" parameters? ")" block ;
logicalOr  -> logicalAnd ( "or" logicalAnd )* ;
logicalOr  -> equality ( "and" equality )* ;
equality   -> comparison ( ( "!=" | "\=\=" ) comparison )\* ;
comparison -> term ( ( ">" | ">=" | "<" | "<=" ) term )\* ;
term       -> factor ( ( "-" | "+" ) factor )\* ;
factor     -> unary ( ( "/" | "\*" | "%" ) unary )\* ;
unary      -> ( "!" | "-" ) unary 
              | primary ;
call       -> primary ( "(" arguments? ")" )* ;
arguments  -> expression ( "," expression )* ;
parameters -> IDENTIFIER ( "," IDENTIFIER )* ;
primary    -> INTEGER | FLOAT | STRING | "true" | "false" | "nil" | ( "("                        expression ")" ) | IDENTIFIER ;
```