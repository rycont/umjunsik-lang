use nom::{
    branch::alt,
    bytes::complete::tag,
    combinator::{all_consuming, map, opt},
    multi::{fold_many0, fold_many1, many0_count, many1_count, separated_list1},
    sequence::{delimited, pair, preceded, terminated, tuple},
    IResult,
};

use super::ast::*;

pub type ParseResult<'a, T> = IResult<&'a str, T>;

pub fn parse_load(s: &str) -> ParseResult<Load> {
    map(many1_count(tag("어")), |count| Load { index: count })(s)
}

fn parse_add_fragment(s: &str) -> ParseResult<i32> {
    map(many1_count(tag(".")), |count| count as i32)(s)
}

fn parse_sub_fragment(s: &str) -> ParseResult<i32> {
    map(many1_count(tag(",")), |count| -(count as i32))(s)
}

fn parse_input_fragment(s: &str) -> ParseResult<usize> {
    many1_count(tag("식?"))(s)
}

enum AddSubOrInput {
    AddSub(i32),
    Input(usize),
}

fn parse_input_or_add_sub(s: &str) -> ParseResult<AddSubOrInput> {
    alt((
        map(parse_add_fragment, AddSubOrInput::AddSub),
        map(parse_sub_fragment, AddSubOrInput::AddSub),
        map(parse_input_fragment, AddSubOrInput::Input),
    ))(s)
}

fn parse_add(s: &str) -> ParseResult<(i32, usize)> {
    fold_many1(
        parse_input_or_add_sub,
        || (0, 0),
        |(add_sub, input), bit| match bit {
            AddSubOrInput::AddSub(a) => (add_sub + a, input),
            AddSubOrInput::Input(i) => (add_sub, input + i),
        },
    )(s)
}

pub fn parse_term(s: &str) -> ParseResult<Term> {
    alt((
        map(pair(opt(parse_load), parse_add), |(load, (add, input))| {
            Term { load, add, input }
        }),
        map(parse_load, |load| Term {
            load: Some(load),
            add: 0,
            input: 0,
        }),
    ))(s)
}

pub fn parse_multiply(s: &str) -> ParseResult<Multiply> {
    map(separated_list1(tag(" "), parse_term), |terms| Multiply {
        terms,
    })(s)
}

pub fn parse_statement(s: &str) -> ParseResult<Statement> {
    alt((
        map(
            tuple((many0_count(tag("어")), tag("엄"), opt(parse_multiply))),
            |(len, _, a)| Statement::Assign {
                index: len + 1,
                value: a,
            },
        ),
        map(delimited(tag("식"), opt(parse_multiply), tag("!")), |int| {
            Statement::PrintInt { value: int }
        }),
        map(
            delimited(tag("식"), opt(parse_multiply), tag("ㅋ")),
            |int| Statement::PrintChar { codepoint: int },
        ),
        map(
            tuple((tag("동탄"), opt(parse_multiply), tag("?"), parse_statement)),
            |(_, int, _, statement)| Statement::If {
                condition: int,
                statement: Box::new(statement),
            },
        ),
        map(preceded(tag("준"), parse_multiply), |int| {
            Statement::Goto { line: int }
        }),
        map(preceded(tag("화이팅!"), opt(parse_multiply)), |int| {
            Statement::Exit { code: int }
        }),
    ))(s)
}

fn parse_newline(s: &str) -> ParseResult<()> {
    map(alt((tag("\n"), tag("\r"), tag("~"))), |_| ())(s)
}

pub fn parse_program(s: &str) -> ParseResult<Program> {
    let (s, _) = terminated(tag("어떻게"), parse_newline)(s)?;
    let (s, mut statements) = fold_many0(
        terminated(opt(parse_statement), parse_newline),
        || vec![None],
        |mut acc, stmt| {
            acc.push(stmt);
            acc
        },
    )(s)?;
    let (s, _) = all_consuming(tag("이 사람이름이냐ㅋㅋ"))(s)?;
    statements.push(None);
    Ok((s, Program { statements }))
}

#[cfg(test)]
mod test {
    use super::*;

    #[test]
    fn load_one() {
        assert_eq!(parse_load("어..").unwrap(), ("..", Load { index: 1 }));
    }

    #[test]
    fn load_many() {
        assert_eq!(parse_load("어어,").unwrap(), (",", Load { index: 2 }));
    }

    #[test]
    fn add_fragment() {
        assert_eq!(parse_add_fragment("......,,.").unwrap(), (",,.", 6));
    }

    #[test]
    fn sub_fragment() {
        assert_eq!(parse_sub_fragment(",,.").unwrap(), (".", -2));
    }

    #[test]
    fn input_fragment() {
        assert_eq!(parse_input_fragment("식?식?").unwrap(), ("", 2));
    }

    #[test]
    fn add() {
        assert_eq!(parse_add("..,.식?,.,,식?,.").unwrap(), ("", (0, 2)));
    }

    #[test]
    fn term_composite() {
        assert_eq!(
            parse_term("어어..,").unwrap(),
            (
                "",
                Term {
                    load: Some(Load { index: 2 }),
                    add: 1,
                    input: 0
                }
            )
        );
    }

    #[test]
    fn term_only_load() {
        assert_eq!(
            parse_term("어어어").unwrap(),
            (
                "",
                Term {
                    load: Some(Load { index: 3 }),
                    add: 0,
                    input: 0
                }
            )
        );
    }

    #[test]
    fn term_only_add() {
        assert_eq!(
            parse_term("..,,,,.").unwrap(),
            (
                "",
                Term {
                    load: None,
                    add: -1,
                    input: 0
                }
            )
        );
    }

    #[test]
    fn multiply_one() {
        assert_eq!(
            parse_multiply("어..식?..").unwrap(),
            (
                "",
                Multiply {
                    terms: vec![Term {
                        load: Some(Load { index: 1 }),
                        add: 4,
                        input: 1
                    }]
                }
            )
        );
    }

    #[test]
    fn multiply_many() {
        assert_eq!(
            parse_multiply(".. , 어어.").unwrap(),
            (
                "",
                Multiply {
                    terms: vec![
                        Term {
                            load: None,
                            add: 2,
                            input: 0
                        },
                        Term {
                            load: None,
                            add: -1,
                            input: 0
                        },
                        Term {
                            load: Some(Load { index: 2 }),
                            add: 1,
                            input: 0
                        },
                    ]
                }
            )
        );
    }

    #[test]
    fn statement_assign() {
        assert_eq!(
            parse_statement("어엄식?").unwrap(),
            (
                "",
                Statement::Assign {
                    index: 2,
                    value: Some(Multiply {
                        terms: vec![Term {
                            load: None,
                            add: 0,
                            input: 1,
                        },],
                    }),
                }
            )
        );
    }

    #[test]
    fn statement_print_int() {
        assert_eq!(
            parse_statement("식어어.. ..!").unwrap(),
            (
                "",
                Statement::PrintInt {
                    value: Some(Multiply {
                        terms: vec![
                            Term {
                                load: Some(Load { index: 2 }),
                                add: 2,
                                input: 0
                            },
                            Term {
                                load: None,
                                add: 2,
                                input: 0
                            },
                        ]
                    })
                }
            )
        );
    }

    #[test]
    fn statement_print_char() {
        assert_eq!(
            parse_statement("식ㅋ").unwrap(),
            ("", Statement::PrintChar { codepoint: None })
        )
    }

    #[test]
    fn statement_if() {
        assert_eq!(
            parse_statement("동탄?동탄.?엄").unwrap(),
            (
                "",
                Statement::If {
                    condition: None,
                    statement: Box::new(Statement::If {
                        condition: Some(Multiply {
                            terms: vec![Term {
                                load: None,
                                add: 1,
                                input: 0
                            }]
                        }),
                        statement: Box::new(Statement::Assign {
                            index: 1,
                            value: None
                        })
                    })
                }
            )
        );
    }

    #[test]
    fn program() {
        let program = "어떻게

엄식?
어엄식?

동탄어?준... ....

엄어,
어엄어어.

준.. ...
식어어!

이 사람이름이냐ㅋㅋ";
        assert_eq!(
            parse_program(program).unwrap(),
            (
                "",
                Program {
                    statements: vec![
                        None,
                        None,
                        Some(Statement::Assign {
                            index: 1,
                            value: Some(Multiply {
                                terms: vec![Term {
                                    load: None,
                                    add: 0,
                                    input: 1
                                }]
                            })
                        }),
                        Some(Statement::Assign {
                            index: 2,
                            value: Some(Multiply {
                                terms: vec![Term {
                                    load: None,
                                    add: 0,
                                    input: 1
                                }]
                            })
                        }),
                        None,
                        Some(Statement::If {
                            condition: Some(Multiply {
                                terms: vec![Term {
                                    load: Some(Load { index: 1 }),
                                    add: 0,
                                    input: 0
                                }]
                            }),
                            statement: Box::new(Statement::Goto {
                                line: Multiply {
                                    terms: vec![
                                        Term {
                                            load: None,
                                            add: 3,
                                            input: 0
                                        },
                                        Term {
                                            load: None,
                                            add: 4,
                                            input: 0
                                        }
                                    ]
                                }
                            })
                        }),
                        None,
                        Some(Statement::Assign {
                            index: 1,
                            value: Some(Multiply {
                                terms: vec![Term {
                                    load: Some(Load { index: 1 }),
                                    add: -1,
                                    input: 0
                                }]
                            })
                        }),
                        Some(Statement::Assign {
                            index: 2,
                            value: Some(Multiply {
                                terms: vec![Term {
                                    load: Some(Load { index: 2 }),
                                    add: 1,
                                    input: 0
                                }]
                            })
                        }),
                        None,
                        Some(Statement::Goto {
                            line: Multiply {
                                terms: vec![
                                    Term {
                                        load: None,
                                        add: 2,
                                        input: 0
                                    },
                                    Term {
                                        load: None,
                                        add: 3,
                                        input: 0
                                    },
                                ]
                            }
                        }),
                        Some(Statement::PrintInt {
                            value: Some(Multiply {
                                terms: vec![Term {
                                    load: Some(Load { index: 2 }),
                                    add: 0,
                                    input: 0,
                                }]
                            })
                        }),
                        None,
                        None,
                    ]
                }
            )
        );
    }
}
