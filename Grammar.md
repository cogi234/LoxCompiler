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
binary -> expression operator expression;
unary -> ( "-" | "!" ) expression ;
grouping -> "(" expression ")" ;
operator -> all the binary operators (\=\=, !=, +, \*, etc.)
literal -> INTEGER | FLOAT | STRING | "true" | "false" | "nil" ;