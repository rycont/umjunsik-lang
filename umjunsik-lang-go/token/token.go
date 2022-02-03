package token

type TokenType string

type Token struct {
	Type    TokenType
	Literal string
}

const (
	ILLEGAL = "ILLEGAL"
	EOF     = "EOF"

	U        = "U"
	UM       = "UM"
	JUN      = "JUN"
	SIK      = "SIK"
	DONGTAN  = "DONGTAN"
	KI       = "KI"
	NEWLINE  = "NEWLINE"
	FIGHTING = "FIGHTING"
	PERIOD   = "."
	COMMA    = ","
	BANG     = "!"
	QUESTION = "?"
	SPACE    = " "
)
