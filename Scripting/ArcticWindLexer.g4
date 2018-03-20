lexer grammar ArcticWindLexer;

// -- Reductions -- //
SET_REDUCTIONS 
	: A V G 
	| C O R R 
	| C O U N T 
	| C O U N T '_' A L L 
	| C O U N T '_' N U L L 
	| C O V A R 
	| F I R S T
	| F R E Q 
	| I N T E R C E P T
	| L A S T 
	| M A X 
	| M I N 
	| S L O P E 
	| S T D E V P
	| S T D E V S
	| S U M 
	| V A R
	;

// Keywods //
K_AS : A S;									
K_ASC : A S C;
K_BY : B Y;
K_CLUSTER : C L U S T E R;
K_CONNECT : C O N N E C T;
K_DESC : D E S C;
K_DO : D O;
K_ELSE : E L S E;
K_ESCAPE : E X I T;
K_EXEC : E X E C | E X E C U T E;
K_FOLD : F O L D;
K_FOR : F O R;
K_FOR_EACH : F O R '_' E A C H | F O R WS E A C H;
K_FROM : F R O M;
K_GROUP : G R O U P;
K_IDENTITY : I D E N T I T Y | I D E N T;
K_IF : I F;
K_IN : I N;
K_INLINE : I N L I N E;
K_MATRIX : M A T R I X;
K_NEW : N E W;
K_ON : O N;
K_OPEN : O P E N;
K_ORDER : O R D E R;
K_PRINT : P R I N T;
K_RECORD : R E C O R D;
K_RETURN : R E T U R N;
K_SCALAR : S C A L A R;
K_SELECT : S E L E C T;
K_TABLE : T A B L E;
K_THEN : T H E N;
K_TO : T O;
K_UNION : U N I O N;
K_VOID : V O I D;
K_WHERE : W H E R E ;
K_WHILE : W H I L E;

// Opperators //
OR : O R | PIPE PIPE;
AND : A N D | AMPER AMPER;
XOR : X O R | POW POW;
NOT : N O T | '!';
PLUS : '+';
MINUS : '-';
MUL : '*';
DIV : '/';
DIV2 : '/?';
MOD : '%';
POW : '^';
EQ : '==';
NEQ : '!=';
LT : '<';
LTE : '<=';
GT : '>';
GTE : '>=';
NULL_OP : '??';
QUESTION : '?';
LPAREN : '(';
RPAREN : ')';
LBRAC : '[';
RBRAC : ']';
LCURL : '{';
RCURL : '}';
COMMA : ',';
SEMI_COLON : ';';
ARROW : '->';
ARROW2 : '=>';
DOT : '.';
ASSIGN : '=';
TILDA : '~';
PIPE : '|';
AMPER : '&';
COLON : ':';
L_SHIFT : '<<';
L_ROTATE : '<<<';
R_SHIFT : '>>';
R_ROTATE : '>>>';


MATRIX_TOK : '$';
RECORD_TOK : '@';
TABLE_TOK : '#';

// Core types //
T_BOOL : B O O L;
T_DATETIME : D A T E T I M E;
T_BYTE : B Y T E;
T_SHORT : S H O R T;
T_INT : I N T;
T_LONG : L O N G;
T_SINGLE : S I N G L E | F L O A T;
T_DOUBLE : D O U B L E | N U M;
T_BINARY : B I N A R Y;
T_BSTRING : B S T R I N G;
T_CSTRING : C S T R I N G;

// Cell Literal Support //
LITERAL_NULL // NULL INT
	: N U L L
	; 
LITERAL_BOOL 
	: T R U E 
	| F A L S E
	;
LITERAL_BINARY
	: '0' X (HEX HEX)*;
LITERAL_DATETIME 
	: '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ '\'' D T 												// 'YYYY-MM-DD'
	| '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ ':' DIGIT+ ':' DIGIT+ ':' DIGIT+ '\'' D T				// 'YYYY-MM-DD:HH:MM:SS'
	| '\'' DIGIT+ '-' DIGIT+ '-' DIGIT+ ':' DIGIT+ ':' DIGIT+ ':' DIGIT+ '.' DIGIT+ '\'' D T	// 'YYYY-MM-DD:HH:MM:SS.LLLLLLLL'
	;
LITERAL_SINGLE
	: DIGIT+ '.' DIGIT+ R  // FLOAT
	| (DIGIT+) R			// 'R' MEANS THIS HAS THE FORM OF AN INT, BUT WE WANT IT TO BE A SINGLE (REAL); AVOIDS HAVING TO DO A CAST
	;
LITERAL_DOUBLE 
	: DIGIT+ '.' DIGIT+ (D)?  // DOUBLE
	| (DIGIT+) D			// 'D' MEANS THIS HAS THE FORM OF AN INT, BUT WE WANT IT TO BE A DOUBLE; AVOIDS HAVING TO DO A CAST
	;
LITERAL_BYTE 
	: DIGIT+ B
	;
LITERAL_SHORT 
	: DIGIT+ S
	;
LITERAL_INT 
	: DIGIT+ (I)?
	;
LITERAL_LONG
	: DIGIT+ L 
	;
LITERAL_BSTRING
	: LITERAL_STRING B;
LITERAL_CSTRING
	: LITERAL_STRING C?;
LITERAL_STRING 
	: '\'' ( ~'\'' | '\'\'' )* '\'' // NORMAL STRING -> 'abcdef'
	| '"' ( ~'"' | '""')* '"'		// NORMAL STRING -> "ABCDEF"
	| '\'\''						// EMPTY STRING -> ''
	| SLIT .*? SLIT					// COMPLEX STRING LITERAL $$ ANYTHING $$
	| C R L F						// \n
	| T A B							// \t
	;

// Base Token //
IDENTIFIER
	: [a-zA-Z_] [a-zA-Z_0-9]*
	;

// Comments and whitespace //
SINGLE_LINE_COMMENT : '//' ~[\r\n]* -> channel(HIDDEN);
MULTILINE_COMMENT : '/*' .*? ( '*/' | EOF ) -> channel(HIDDEN);
WS : ( ' ' | '\t' |'\r' | '\n' | '\r\n')* -> channel(HIDDEN);

fragment SLIT : '$$';
fragment DIGIT : [0-9];
fragment HEX : [aAbBcCdDeEfF0123456789];
fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];
