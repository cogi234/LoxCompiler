## Notation:
nonterminal -> "terminal" nonterminal LITERAL_TERMINAL ;
| : or
() : grouping
\* : repeat 0+ times
\+ : repeat 1+ times
? : optional
## Statements
program -> statement\* EOF ;
declaration -> variableDeclaration | statement ;
variableDeclaration -> "var" IDENTIFIER ( "=" expression )? ";" ; 
statement -> expressionStatement | printStatement | ifStatement | block ;
ifStatement -> "if" "(" expression ")" statement ( "else" statement)? ;
expressionStatement -> expression ";" ;
printStatement -> "print" expression ";" ;
## Expressions
expression -> assignment ;
assigment -> IDENTIFIER "=" assignment
			| logicalOr;
logicalOr -> logicalAnd ( "or" logicalAnd )* ;
logicalOr -> equality ( "and" equality )* ;
equality -> comparison ( ( "!=" | "\=\=" ) comparison )\* ;
comparison -> term ( ( ">" | ">=" | "<" | "<=" ) term )\* ;
term -> factor ( ( "-" | "+" ) factor )\* ;
factor -> unary ( ( "/" | "\*" | "%" ) unary )\* ;
unary -> ( "!" | "-" ) unary 
		| primary ;
primary -> INTEGER | FLOAT | STRING | "true" | "false" | "nil" | ( "(" expression ")" ) | IDENTIFIER ;