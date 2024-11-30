## Notation:
nonterminal -> "terminal" nonterminal LITERAL_TERMINAL ;
| : or
() : grouping
\* : repeat 0+ times
\+ : repeat 1+ times
? : optional

## Expressions
expression -> literal
			| unary
			| binary
			| grouping ;
literal -> INTEGER | FLOAT | STRING | "true" | "false" | "nil" ;
grouping -> "(" expression ")" ;
unary -> ( "-" | "!" ) expression ;
binary -> expression operator expression;
operator -> all the binary operators (\=\=, !=, +, \*, etc.)