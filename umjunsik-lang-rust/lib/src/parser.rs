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
    map(many1_count(tag("어")), Load)(s)
}

fn parse_add_fragment(s: &str) -> ParseResult<i32> {
    map(many1_count(tag(".")), |count| count as i32)(s)
}

fn parse_sub_fragment(s: &str) -> ParseResult<i32> {
    map(many1_count(tag(",")), |count| -(count as i32))(s)
}

fn parse_add(s: &str) -> ParseResult<i32> {
    fold_many1(
        alt((parse_add_fragment, parse_sub_fragment)),
        || 0,
        |acc, frag| acc + frag,
    )(s)
}

pub fn parse_term(s: &str) -> ParseResult<Term> {
    alt((
        map(pair(opt(parse_load), parse_add), |(load, add)| {
            Term(load, add)
        }),
        map(parse_load, |load| Term(Some(load), 0)),
    ))(s)
}

pub fn parse_multiply(s: &str) -> ParseResult<Multiply> {
    map(separated_list1(tag(" "), parse_term), Multiply)(s)
}

pub fn parse_int(s: &str) -> ParseResult<Int> {
    alt((map(parse_multiply, Int::Pure), map(tag("식?"), |_| Int::IO)))(s)
}

pub fn parse_statement(s: &str) -> ParseResult<Statement> {
    alt((
        map(
            tuple((many0_count(tag("어")), tag("엄"), opt(parse_int))),
            |(len, _, a)| Statement::Assign(len + 1, a),
        ),
        map(
            delimited(tag("식"), opt(parse_multiply), tag("!")),
            Statement::PrintInt,
        ),
        map(
            delimited(tag("식"), opt(parse_multiply), tag("ㅋ")),
            Statement::PrintChar,
        ),
        map(
            tuple((tag("동탄"), opt(parse_multiply), tag("?"), parse_statement)),
            |(_, int, _, statement)| Statement::If(int, Box::new(statement)),
        ),
        map(preceded(tag("준"), opt(parse_multiply)), Statement::Goto),
        map(
            preceded(tag("화이팅!"), opt(parse_multiply)),
            Statement::Exit,
        ),
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
    Ok((s, Program(statements)))
}

#[cfg(test)]
mod test {
    use super::*;

    #[test]
    fn load_one() {
        assert_eq!(parse_load("어..").unwrap(), ("..", Load(1)));
    }

    #[test]
    fn load_many() {
        assert_eq!(parse_load("어어,").unwrap(), (",", Load(2)));
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
    fn add() {
        assert_eq!(parse_add("..,.,.,,,.").unwrap(), ("", 0));
    }

    #[test]
    fn term_composite() {
        assert_eq!(parse_term("어어..,").unwrap(), ("", Term(Some(Load(2)), 1)));
    }

    #[test]
    fn term_only_load() {
        assert_eq!(parse_term("어어어").unwrap(), ("", Term(Some(Load(3)), 0)));
    }

    #[test]
    fn term_only_add() {
        assert_eq!(parse_term("..,,,,.").unwrap(), ("", Term(None, -1)));
    }

    #[test]
    fn multiply_one() {
        assert_eq!(
            parse_multiply("어....").unwrap(),
            ("", Multiply(vec![Term(Some(Load(1)), 4)]))
        );
    }

    #[test]
    fn multiply_many() {
        assert_eq!(
            parse_multiply(".. , 어어.").unwrap(),
            (
                "",
                Multiply(vec![Term(None, 2), Term(None, -1), Term(Some(Load(2)), 1)])
            )
        );
    }

    #[test]
    fn int_pure() {
        assert_eq!(
            parse_int(". .").unwrap(),
            ("", Int::Pure(Multiply(vec![Term(None, 1), Term(None, 1)])))
        );
    }

    #[test]
    fn int_io() {
        assert_eq!(parse_int("식?").unwrap(), ("", Int::IO));
    }

    #[test]
    fn statement_assign() {
        assert_eq!(
            parse_statement("어엄식?").unwrap(),
            ("", Statement::Assign(2, Some(Int::IO)))
        );
    }

    #[test]
    fn statement_print_int() {
        assert_eq!(
            parse_statement("식어어.. ..!").unwrap(),
            (
                "",
                Statement::PrintInt(Some(Multiply(vec![Term(Some(Load(2)), 2), Term(None, 2)])))
            )
        );
    }

    #[test]
    fn statement_print_char() {
        assert_eq!(
            parse_statement("식ㅋ").unwrap(),
            ("", Statement::PrintChar(None),)
        )
    }

    #[test]
    fn statement_if() {
        assert_eq!(
            parse_statement("동탄?동탄.?엄").unwrap(),
            (
                "",
                Statement::If(
                    None,
                    Box::new(Statement::If(
                        Some(Multiply(vec![Term(None, 1)])),
                        Box::new(Statement::Assign(1, None))
                    ))
                )
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
                Program(vec![
                    None,
                    None,
                    Some(Statement::Assign(1, Some(Int::IO))),
                    Some(Statement::Assign(2, Some(Int::IO))),
                    None,
                    Some(Statement::If(
                        Some(Multiply(vec![Term(Some(Load(1)), 0)])),
                        Box::new(Statement::Goto(Some(Multiply(vec![
                            Term(None, 3),
                            Term(None, 4)
                        ]))))
                    )),
                    None,
                    Some(Statement::Assign(
                        1,
                        Some(Int::Pure(Multiply(vec![Term(Some(Load(1)), -1)])))
                    )),
                    Some(Statement::Assign(
                        2,
                        Some(Int::Pure(Multiply(vec![Term(Some(Load(2)), 1)])))
                    )),
                    None,
                    Some(Statement::Goto(Some(Multiply(vec![
                        Term(None, 2),
                        Term(None, 3)
                    ])))),
                    Some(Statement::PrintInt(Some(Multiply(vec![Term(
                        Some(Load(2)),
                        0
                    )])))),
                    None,
                    None,
                ])
            )
        );
    }
}
