grammar MathExpr;

statements
	: statement (';' statement?)*
	;

statement
	: expression ('=' expression)?
	;

expression
	: op=(Plus | Minus) expression                 #unaryExpression

	| <assoc=right> expression Caret expression    #binaryExpression
	| expression op=(Mult | Div) expression        #binaryExpression
	| expression op=(Plus | Minus) expression      #binaryExpression

	| '(' expression ')' Quote?                    #parenthesisExpression
	| '|' expression '|' Quote?                    #absoluteExpression

	| Id Quote? '(' expressionList ')'             #funcExpression
	| Id '(' expressionList ')' Quote              #funcExpression

	| Id                                           #idExpression
	| number                                       #numberExpression
	;

expressionList
	: expression (',' expression)*
	;

number
	: intPart=Digits fracTail?
	| fracTail
	;

fracTail
	: '.' fracPart=Digits ('(' periodPart=Digits ')')?
	| '.' '(' periodPart=Digits ')'
	;

Id          : Letter (Letter | Digit)*;
Digits      : Digit+;
Mult        : '*';
Div         : '/';
Plus        : '+';
Minus       : '-';
Separator   : ';';
Equal       : '=';
Quote       : '\'';
LParen      : '(';
RParen      : ')';
VBar        : '|';
Caret       : '^';
Dot         : '.';
Comma       : ',';

Whitespace  : [ \t\r\n]+ -> channel(HIDDEN);

fragment Letter: [a-zA-Z];
fragment Digit:  [0-9];