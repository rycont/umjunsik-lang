package object

import "strconv"

type Environment struct {
	store map[int]Object
}

func NewEnvironment() *Environment {
	s := make(map[int]Object)
	return &Environment{store: s}
}

func (env *Environment) Get(index int) (Object, bool) {
	obj, ok := env.store[index]
	if !ok {
		obj = &Integer{Value: 0}
	}
	return obj, true
}

func (env *Environment) Set(index int, obj Object) {
	env.store[index] = obj
}

const (
	JUN_OBJ     = "JUN_OBJ"
	INTEGER_OBJ = "INTEGER_OBJ"
	ERROR_OBJ   = "ERROR_OBJ"
)

type ObjectType string

type Object interface {
	Type() ObjectType
	Inspect() string
}

type JUN struct {
	Index int64
}

func (j *JUN) Type() ObjectType { return JUN_OBJ }
func (j *JUN) Inspect() string  { return strconv.Itoa(int(j.Index)) }

type Integer struct {
	Value int64
}

func (i *Integer) Type() ObjectType { return INTEGER_OBJ }
func (i *Integer) Inspect() string  { return strconv.Itoa(int(i.Value)) }

type Error struct {
	Message string
}

func (e *Error) Type() ObjectType { return ERROR_OBJ }
func (e *Error) Inspect() string  { return "ERROR: " + e.Message }
