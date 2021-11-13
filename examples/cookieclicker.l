MPTO 3
MVAR "cookies" 0
MPTO 0
MVAR "cookiesBKP" 0

STARTGOTO "loop"

MPTO 3

CPR "Cookies:"
CPRVAR "cookies"

MPTO 1
GI:VAR "dummy"
MPTO 2
MVAR "one" 1

MPTO 3
ADD 2 0 "cookies"
MPTO 0
SET 0 3

MPTO 0
GOTO

ENDGOTO

MPTO 0
GOTO
