grammar umm;

@header {
}
program: START (NEWLINE | statements)* END;

statements
    : statement NEWLINE
    ;

statement
    : varAssign
    | varAccess
    | consoleOutput
    | conditional
    | jump
    | programEnd
    ;

varAssign: VAR_ASSIGN (INTEGER?|CONSOLE_INPUT|(VAR_ACCESS INTEGER*));
varAccess: VAR_ACCESS;
consoleOutput: SIK (VAR_ACCESS | INTEGER) EXCLAMATION;
conditional: DONGTAN (VAR_ACCESS | INTEGER) QUESTION statement;
jump: JUN INTEGER;
programEnd: '화이팅!' INTEGER;

// tokens
EXCLAMATION: '!';
QUESTION: '?';
SPACE: ' ';
LAUGH: 'ㅋ';
JUN: '준';
SIK: '식';
DONGTAN: '동탄';
START: '어떻게';
END: '이 사람이름이냐ㅋㅋ';
CONSOLE_INPUT : '식?';

INTEGER: (DOT | COMMA)+ | ((DOT | COMMA)+ SPACE INTEGER);

VAR_ASSIGN: UH* UM;
VAR_ACCESS: UH+;

WS: [ \t]+ -> skip;
NEWLINE: ( '\r'? '\n' | '\r' | '\f' ) SPACE?;

//fragments
fragment UH: '어';
fragment UM: '엄';
fragment DOT: '.';
fragment COMMA: ',';
