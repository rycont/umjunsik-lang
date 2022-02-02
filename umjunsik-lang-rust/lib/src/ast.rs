#[derive(Debug, PartialEq)]
pub struct Load(pub usize);

#[derive(Debug, PartialEq)]
pub struct Term(pub Option<Load>, pub i32);

#[derive(Debug, PartialEq)]
pub struct Multiply(pub Vec<Term>);

pub type PureInt = Multiply;

#[derive(Debug, PartialEq)]
pub enum Int {
    Pure(PureInt),
    IO,
}

#[derive(Debug, PartialEq)]
pub enum Statement {
    Assign(usize, Option<Int>),
    PrintInt(Option<PureInt>),
    PrintChar(Option<PureInt>),
    If(Option<PureInt>, Box<Statement>),
    Goto(Option<PureInt>),
    Exit(Option<PureInt>),
}

#[derive(Debug, PartialEq)]
pub struct Program(pub Vec<Option<Statement>>);
