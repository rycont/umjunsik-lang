package parser

import (
	"umjunsik-lang/umjunsik-lang-go/ast"
	"umjunsik-lang/umjunsik-lang-go/lexer"
	"umjunsik-lang/umjunsik-lang-go/token"
)

type Parser struct {
	l      *lexer.Lexer
	errors []string

	curToken  token.Token
	peekToken token.Token

	prefixParseFns map[token.TokenType]prefixParseFn
	infixParseFns  map[token.TokenType]infixParseFn
}

type (
	prefixParseFn func() ast.Expression
	infixParseFn  func(ast.Expression) ast.Expression
)

func (p *Parser) registerPrefix(tokenType token.TokenType, fn prefixParseFn) {
	p.prefixParseFns[tokenType] = fn
}

func (p *Parser) registerInfix(tokenType token.TokenType, fn infixParseFn) {
	p.infixParseFns[tokenType] = fn
}

func New(l *lexer.Lexer) *Parser {
	p := &Parser{
		l:      l,
		errors: []string{},
	}
	p.nextToken()
	p.nextToken()

	p.prefixParseFns = make(map[token.TokenType]prefixParseFn)
	p.registerPrefix(token.U, p.parseUExpression)
	p.registerPrefix(token.PERIOD, p.parsePrefixIntegerExpression)
	p.registerPrefix(token.COMMA, p.parsePrefixIntegerExpression)
	p.registerPrefix(token.UM, p.parseUMExpression)
	p.registerPrefix(token.JUN, p.parseJUNExpression)
	p.registerPrefix(token.SIK, p.parseSIKExpression)
	p.registerPrefix(token.DONGTAN, p.parseDONGTANExpression)
	p.registerPrefix(token.FIGHTING, p.parseFIGHTINGExpression)

	p.infixParseFns = make(map[token.TokenType]infixParseFn)
	p.registerInfix(token.PERIOD, p.parseInfixIntegerExpression)
	p.registerInfix(token.COMMA, p.parseInfixIntegerExpression)
	p.registerInfix(token.SPACE, p.parseSPACEExpression)

	return p
}

func (p *Parser) ParseProgram() *ast.Program {
	program := &ast.Program{}
	program.Lines = []ast.Line{}

	for p.curToken.Type != token.EOF {
		line := p.parseLine()
		program.Lines = append(program.Lines, line)

		p.nextToken()
	}

	return program
}

func (p *Parser) parseLine() ast.Line {
	return p.parseExpressionStatement()
}

func (p *Parser) parseExpressionStatement() *ast.ExpressionLine {
	line := &ast.ExpressionLine{Token: p.curToken}
	line.Expression = p.parseExpression()

	if p.peekTokenIs(token.NEWLINE) {
		p.nextToken()
	}

	return line
}

func (p *Parser) parseExpression() ast.Expression {
	prefix := p.prefixParseFns[p.curToken.Type]
	if prefix == nil {
		p.noPrefixParseFnError(p.curToken.Type)
		return nil
	}
	leftExp := prefix()

	for !(p.peekTokenIs(token.NEWLINE) || p.peekTokenIs(token.KI) || p.peekTokenIs(token.QUESTION) || p.peekTokenIs(token.BANG)) {
		infix := p.infixParseFns[p.peekToken.Type]
		if infix == nil {
			return leftExp
		}

		p.nextToken()

		leftExp = infix(leftExp)
	}

	return leftExp
}

func (p *Parser) parseUExpression() ast.Expression {
	exp := &ast.UEpxression{Token: p.curToken}

	var index int64 = 0

	for p.peekTokenIs(token.U) {
		index++
		p.nextToken()
	}

	if p.peekTokenIs(token.UM) {
		p.nextToken()
		exp := &ast.UMExpression{Token: p.curToken}
		exp.Index = index + 1

		if p.peekTokenIs(token.NEWLINE) {
			if exp.Value == nil {
				exp.Value = &ast.IntegerLiteral{
					Token: token.Token{
						Type:    token.PERIOD,
						Literal: ".",
					},
					Value: 0,
				}
			}
		} else {
			p.nextToken()
			exp.Value = p.parseExpression()
		}

		return exp
	}

	exp.Index = index

	return exp
}

func (p *Parser) parseUMExpression() ast.Expression {
	if p.curTokenIs(token.UM) {
		p.nextToken()
	}

	tmp := p.parseExpression()
	tok := p.curToken

	if tmp != nil {
		return &ast.UMExpression{Token: tok, Index: 0, Value: tmp}
	} else {
		return &ast.UMExpression{Token: tok, Index: 0, Value: &ast.IntegerLiteral{
			Token: token.Token{
				Type:    token.PERIOD,
				Literal: ".",
			},
			Value: 0,
		},
		}
	}
}

func (p *Parser) parsePrefixIntegerExpression() ast.Expression {
	lit := &ast.IntegerLiteral{Token: p.curToken}

	var result int64 = 0

	if p.curTokenIs(token.PERIOD) {
		result++
	} else if p.curTokenIs(token.COMMA) {
		result--
	}

	for p.peekTokenIs(token.PERIOD) || p.peekTokenIs(token.COMMA) {
		switch p.peekToken.Type {
		case token.PERIOD:
			result++
		case token.COMMA:
			result--
		}
		p.nextToken()
	}

	lit.Value = result

	return lit
}

func (p *Parser) parseJUNExpression() ast.Expression {
	exp := &ast.JUNEExpression{Token: p.curToken}

	if p.peekTokenIs(token.NEWLINE) {
		return nil
	}

	p.nextToken()

	exp.Index = p.parseExpression()

	return exp
}

func (p *Parser) parseSIKExpression() ast.Expression {
	if p.peekTokenIs(token.QUESTION) {
		p.nextToken()
		return &ast.SIKQExpression{Token: p.curToken}
	}

	p.nextToken()

	// if p.peekTokenIs(token.BANG) {
	//	p.nextToken()
	//}
	if p.curTokenIs(token.KI) {
		exp := &ast.SIKKIExpression{Token: p.curToken}
		exp.Value = &ast.IntegerLiteral{
			Token: token.Token{
				Type:    token.PERIOD,
				Literal: ".",
			},
			Value: 10,
		}

		return exp
	}

	tem := p.parseExpression()

	switch p.peekToken.Type {
	case token.BANG:
		p.nextToken()
		exp := &ast.SIKBExpression{Token: p.curToken}

		exp.Value = tem
		return exp
	case token.KI:
		p.nextToken()
		exp := &ast.SIKKIExpression{Token: p.curToken}
		if tem != nil {
			exp.Value = tem
		}
		return exp
	}

	return nil
}

func (p *Parser) parseDONGTANExpression() ast.Expression {
	exp := &ast.DONGTANExpression{Token: p.curToken}

	if p.peekTokenIs(token.NEWLINE) {
		return nil
	}

	p.nextToken()

	exp.Condition = p.parseExpression()

	if !p.expectPeek(token.QUESTION) {
		return nil
	}

	p.nextToken()

	exp.Statement = p.parseExpression()

	return exp
}

func (p *Parser) parseFIGHTINGExpression() ast.Expression {
	exp := &ast.FIGHTINGExpression{Token: p.curToken}

	if !p.expectPeek(token.BANG) {
		return nil
	}

	p.nextToken()

	exp.Value = p.parseExpression()

	return exp
}

func (p *Parser) parseInfixIntegerExpression(left ast.Expression) ast.Expression {
	exp := &ast.InfixIntegerExpression{Token: p.curToken, Left: left}

	var result int64 = 0

	if p.curTokenIs(token.PERIOD) {
		result++
	} else if p.curTokenIs(token.COMMA) {
		result--
	}

	for p.peekTokenIs(token.PERIOD) || p.peekTokenIs(token.COMMA) {
		switch p.curToken.Type {
		case token.PERIOD:
			result++
		case token.COMMA:
			result--
		}
		p.nextToken()
	}

	exp.Right = result

	return exp
}

func (p *Parser) parseSPACEExpression(left ast.Expression) ast.Expression {
	exp := &ast.SPACEExpression{Token: p.curToken, Left: left}

	p.nextToken()
	exp.Right = p.parseExpression()

	return exp
}
