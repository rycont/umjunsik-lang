#[derive(Debug, PartialEq)]
pub struct Load {
    pub index: usize,
}

#[derive(Debug, PartialEq)]
pub struct Term {
    pub load: Option<Load>,
    pub add: i32,
    pub input: usize,
}

#[derive(Debug, PartialEq)]
pub struct Multiply {
    pub terms: Vec<Term>,
}

#[derive(Debug, PartialEq)]
pub enum Statement {
    Assign {
        index: usize,
        value: Option<Multiply>,
    },
    PrintInt {
        value: Option<Multiply>,
    },
    PrintChar {
        codepoint: Option<Multiply>,
    },
    If {
        condition: Option<Multiply>,
        statement: Box<Statement>,
    },
    Goto {
        line: Multiply,
    },
    Exit {
        code: Option<Multiply>,
    },
}

#[derive(Debug, PartialEq)]
pub struct Program {
    pub statements: Vec<Option<Statement>>,
}
