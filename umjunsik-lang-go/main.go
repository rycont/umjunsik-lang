package main

import (
	"flag"
	"io/ioutil"
	"umjunsik-lang/umjunsik-lang-go/eval"
	"umjunsik-lang/umjunsik-lang-go/lexer"
	"umjunsik-lang/umjunsik-lang-go/object"
	"umjunsik-lang/umjunsik-lang-go/parser"
)

var (
	FilePath string
)

func init() {
	flag.Parse()
	FilePath = flag.Args()[0]
}

func main() {
	Code, err := ioutil.ReadFile(FilePath)
	if err != nil {
		panic(err)
	}
	Interpret(Code)
}

func Interpret(Code []byte) {
	l := lexer.New(string(Code))
	p := parser.New(l)
	program := p.ParseProgram()
	env := object.NewEnvironment()
	eval.Eval(program, env)

	// for tok := l.NextToken(); tok.Type != token.EOF; tok = l.NextToken() {
	//	fmt.Println("{" + " Type : " + string(tok.Type) + ", Literal : " + tok.Literal + " }")
	//}
}
