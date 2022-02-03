package ast

import (
	"bytes"
	"strconv"
	"umjunsik-lang/umjunsik-lang-go/token"
)

type Node interface {
	TokenLiteral() string
	String() string
}

type Line interface {
	Node
	lineNode()
}

type Expression interface {
	Node
	expressionNode()
}

type Program struct {
	Lines []Line
}

func (p *Program) TokenLiteral() string {
	if len(p.Lines) > 0 {
		return p.Lines[0].TokenLiteral()
	} else {
		return ""
	}
}

func (p *Program) String() string {
	var out bytes.Buffer

	for i, s := range p.Lines {
		out.WriteString(strconv.Itoa(i) + ". ")
		out.WriteString(s.String())
		out.WriteString("\n")
	}

	return out.String()
}

type ExpressionLine struct {
	Token      token.Token
	Expression Expression
}

func (el *ExpressionLine) lineNode()            {}
func (el *ExpressionLine) TokenLiteral() string { return el.Token.Literal }
func (el *ExpressionLine) String() string {
	if el.Expression != nil {
		return el.Expression.String()
	}
	return ""
}

type InfixIntegerExpression struct {
	Token token.Token
	Left  Expression
	Right int64
}

func (ie *InfixIntegerExpression) expressionNode()      {}
func (ie *InfixIntegerExpression) TokenLiteral() string { return ie.Token.Literal }
func (ie *InfixIntegerExpression) String() string {
	var out bytes.Buffer

	out.WriteString("(")
	out.WriteString(ie.Left.String())
	out.WriteString(" + " + strconv.Itoa(int(ie.Right)))
	out.WriteString(")")

	return out.String()
}

type SPACEExpression struct {
	Token token.Token
	Left  Expression
	Right Expression
}

func (se *SPACEExpression) expressionNode()      {}
func (se *SPACEExpression) TokenLiteral() string { return se.Token.Literal }
func (se *SPACEExpression) String() string {
	var out bytes.Buffer

	out.WriteString("(")
	out.WriteString(se.Left.String())
	out.WriteString(" * " + se.Right.String())
	out.WriteString(")")

	return out.String()
}

type IntegerLiteral struct {
	Token token.Token
	Value int64
}

func (il *IntegerLiteral) expressionNode()      {}
func (il *IntegerLiteral) TokenLiteral() string { return il.Token.Literal }
func (il *IntegerLiteral) String() string {
	var out bytes.Buffer

	for i := 0; i < int(il.Value); i++ {
		out.WriteString(".")
	}

	for i := 0; i > int(il.Value); i-- {
		out.WriteString(",")
	}

	return out.String()
}

type UEpxression struct {
	Token token.Token
	Index int64
}

func (ue *UEpxression) expressionNode()      {}
func (ue *UEpxression) TokenLiteral() string { return ue.Token.Literal }
func (ue *UEpxression) String() string {
	var out bytes.Buffer

	for i := 0; i < int(ue.Index+1); i++ {
		out.WriteString("어")
	}

	return out.String()
}

type UMExpression struct {
	Token token.Token
	Index int64
	Value Expression
}

func (ume *UMExpression) expressionNode()      {}
func (ume *UMExpression) TokenLiteral() string { return ume.Token.Literal }
func (ume *UMExpression) String() string {
	var out bytes.Buffer

	for i := 0; i < int(ume.Index); i++ {
		out.WriteString("어")
	}

	out.WriteString("엄")
	out.WriteString(ume.Value.String())

	return out.String()
}

type JUNEExpression struct {
	Token token.Token
	Index Expression
}

func (je *JUNEExpression) expressionNode()      {}
func (je *JUNEExpression) TokenLiteral() string { return je.Token.Literal }
func (je *JUNEExpression) String() string {
	var out bytes.Buffer

	out.WriteString("준")

	out.WriteString(je.Index.String())

	return out.String()
}

type SIKQExpression struct {
	Token token.Token
}

func (sqe *SIKQExpression) expressionNode()      {}
func (sqe *SIKQExpression) TokenLiteral() string { return sqe.Token.Literal }
func (sqe *SIKQExpression) String() string {
	return "식?"
}

type SIKKIExpression struct {
	Token token.Token
	Value Expression
}

func (ske *SIKKIExpression) expressionNode()      {}
func (ske *SIKKIExpression) TokenLiteral() string { return ske.Token.Literal }
func (ske *SIKKIExpression) String() string {
	var out bytes.Buffer

	out.WriteString("식")
	out.WriteString(ske.Value.String())
	out.WriteString("ㅋ")

	return out.String()
}

type SIKBExpression struct {
	Token token.Token
	Value Expression
}

func (sbe *SIKBExpression) expressionNode()      {}
func (sbe *SIKBExpression) TokenLiteral() string { return sbe.Token.Literal }
func (sbe *SIKBExpression) String() string {
	var out bytes.Buffer

	out.WriteString("식")
	out.WriteString(sbe.Value.String())
	out.WriteString("!")

	return out.String()
}

type DONGTANExpression struct {
	Token     token.Token
	Condition Expression
	Statement Expression
}

func (de *DONGTANExpression) expressionNode()      {}
func (de *DONGTANExpression) TokenLiteral() string { return de.Token.Literal }
func (de *DONGTANExpression) String() string {
	var out bytes.Buffer

	out.WriteString("동탄")
	out.WriteString(de.Condition.String())
	out.WriteString("?")
	out.WriteString(de.Statement.String())

	return out.String()
}

type FIGHTINGExpression struct {
	Token token.Token
	Value Expression
}

func (fe *FIGHTINGExpression) expressionNode()      {}
func (fe *FIGHTINGExpression) TokenLiteral() string { return fe.Token.Literal }
func (fe *FIGHTINGExpression) String() string {
	var out bytes.Buffer

	out.WriteString("화이팅!")
	out.WriteString(fe.Value.String())

	return out.String()
}
