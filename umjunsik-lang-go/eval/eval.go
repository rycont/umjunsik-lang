package eval

import (
	"fmt"
	"os"
	"umjunsik-lang/umjunsik-lang-go/ast"
	"umjunsik-lang/umjunsik-lang-go/object"
)

func Eval(node ast.Node, env *object.Environment) object.Object {
	switch node := node.(type) {
	case *ast.Program:
		return evalProgram(node, env)
	case *ast.ExpressionLine:
		return Eval(node.Expression, env)
	case *ast.InfixIntegerExpression:
		leftObj := Eval(node.Left, env)
		return &object.Integer{Value: leftObj.(*object.Integer).Value + node.Right}
	case *ast.SPACEExpression:
		leftObj := Eval(node.Left, env)
		rightObj := Eval(node.Right, env)
		return &object.Integer{Value: leftObj.(*object.Integer).Value * rightObj.(*object.Integer).Value}
	case *ast.IntegerLiteral:
		return &object.Integer{Value: node.Value}
	case *ast.UEpxression:
		obj, ok := env.Get(int(node.Index))
		if !ok {
			return newError("ObjectFindError")
		}

		return obj
	case *ast.UMExpression:
		obj := Eval(node.Value, env)
		env.Set(int(node.Index), obj)
	case *ast.JUNEExpression:
		obj := Eval(node.Index, env)
		return &object.JUN{Index: obj.(*object.Integer).Value}
	case *ast.SIKQExpression:
		var integer int
		fmt.Scanln(&integer)
		return &object.Integer{Value: int64(integer)}
	case *ast.SIKKIExpression:
		obj := Eval(node.Value, env)
		fmt.Printf("%c", obj.(*object.Integer).Value)
	case *ast.SIKBExpression:
		obj := Eval(node.Value, env)
		fmt.Printf("%d", obj.(*object.Integer).Value)
	case *ast.DONGTANExpression:
		conditionObj := Eval(node.Condition, env)
		if conditionObj.(*object.Integer).Value == 0 {
			return Eval(node.Statement, env)
		}
	case *ast.FIGHTINGExpression:
		obj := Eval(node.Value, env)
		os.Exit(int(obj.(*object.Integer).Value))
	}

	return nil
}

func evalProgram(program *ast.Program, env *object.Environment) object.Object {
	var result object.Object

	for i := 0; i < len(program.Lines); i++ {
		line := program.Lines[i]

		result = Eval(line, env)

		switch result := result.(type) {
		case *object.JUN:
			i = int(result.Index - 3)
		case *object.Error:
			return result
		}
	}

	return result
}
