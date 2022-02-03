package lexer

import (
	"umjunsik-lang/umjunsik-lang-go/token"
)

type Lexer struct {
	input        string
	position     int
	readPosition int
	ch           string
}

func New(input string) *Lexer {
	l := &Lexer{input: input}
	l.readChar()
	if l.ch == "어" {
		l.readChar()
		if l.ch == "떻" {
			l.readChar()
			if l.ch == "게" {
				l.readChar()
			} else {
				return nil
			}
		} else {
			return nil
		}
	} else {
		return nil
	}
	return l
}

func (l *Lexer) NextToken() token.Token {
	var tok token.Token

	l.skipWhiteSpace()

	switch l.ch {
	case "어":
		tok = newToken(token.U, "어")
	case "엄":
		tok = newToken(token.UM, "엄")
	case "준":
		tok = newToken(token.JUN, "준")
	case "식":
		tok = newToken(token.SIK, "식")
	case "동":
		if l.peekchar() == "탄" {
			tok = newToken(token.DONGTAN, "동탄")
			l.readChar()
		}
	case "ㅋ":
		tok = newToken(token.KI, "ㅋ")
	case "화":
		if l.peekchar() == "이" {
			l.readChar()
			if l.peekchar() == "팅" {
				l.readChar()
				tok = newToken(token.FIGHTING, "화이팅")
			}
		}
	case ".":
		tok = newToken(token.PERIOD, ".")
	case ",":
		tok = newToken(token.COMMA, ",")
	case "?":
		tok = newToken(token.QUESTION, "?")
	case "!":
		tok = newToken(token.BANG, "!")
	case "~":
		tok = newToken(token.NEWLINE, "~")
	case "\n":
		tok = newToken(token.NEWLINE, "\n")
	case " ":
		tok = newToken(token.SPACE, " ")
	case "이":
		if l.IsLast() {
			tok = newToken(token.EOF, "")
			return tok
		}
	}

	l.readChar()
	return tok
}

func newToken(tokenType token.TokenType, ch string) token.Token {
	return token.Token{Type: tokenType, Literal: ch}
}

// UTF-8 인코딩에 따라 문자를 읽습니다.
func (l *Lexer) readChar() {
	Byte1 := l.input[l.readPosition]

	if Byte1 >= 128 {
		l.ch = string([]byte{Byte1, l.input[l.readPosition+1], l.input[l.readPosition+2]})

		l.position = l.readPosition
		l.readPosition += 3
	} else {
		l.ch = string(Byte1)
		l.position = l.readPosition
		l.readPosition += 1
	}
}

func (l *Lexer) peekchar() string {
	if l.input[l.readPosition] >= 128 {
		return string([]byte{l.input[l.readPosition], l.input[l.readPosition+1], l.input[l.readPosition+2]})
	} else {
		return string(l.input[l.readPosition])
	}
}

func (l *Lexer) skipWhiteSpace() {
	if l.ch == "\r" || l.ch == "\t" || l.ch == "\v" || l.ch == "\f" {
		l.readChar()
	}
}

func (l *Lexer) IsLast() bool {
	return l.input[l.position:l.position+28] == "이 사람이름이냐ㅋㅋ"
}
