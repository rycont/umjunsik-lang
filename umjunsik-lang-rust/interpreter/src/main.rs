use std::{collections::HashMap, fmt::Display, io::Write};

use clap::Parser;
use umjunsik::{
    ast::{Load, Multiply, Statement},
    parser::parse_statement,
};
use whiteread::Reader;

/// Umjunsik-lang REPL / Interpreter
#[derive(Parser, Debug)]
#[clap(author, version, about)]
struct Args {
    /// Name of source file
    input: Option<String>,
}

fn main() {
    let args = Args::parse();

    let exit_code = if let Some(input) = args.input {
        let code = std::fs::read_to_string(&input).expect("파일을 읽는 도중 오류가 발생했습니다.");
        interpret(code.trim())
    } else {
        repl().expect("REPL 실행 중 오류가 발생했습니다.")
    };
    std::process::exit(exit_code);
}

fn repl() -> std::io::Result<i32> {
    let stdin = std::io::stdin();
    let mut ctx = Context {
        len: 0,
        pc: 0,
        vars: HashMap::new(),
        exit: None,
        reader: Reader::new(stdin.lock()),
    };
    let mut input = String::new();
    while ctx.exit.is_none() {
        print!("> ");
        std::io::stdout().flush()?;
        input.clear();
        let read = std::io::stdin().read_line(&mut input)?;
        if read == 0 {
            break;
        }
        if let Ok((_, statement)) = parse_statement(&input) {
            if let Err(e) = run_statement(&mut ctx, &statement) {
                eprintln!("{}", e);
            }
            println!();
        } else {
            eprintln!("코드의 문법이 맞지 않습니다.");
        }
    }
    Ok(ctx.exit.unwrap_or(0))
}

fn interpret(code: &str) -> i32 {
    let (_, program) = umjunsik::parser::parse_program(code).expect("코드의 문법이 맞지 않습니다.");

    let stdin = std::io::stdin();
    let mut ctx = Context {
        len: program.statements.len(),
        pc: 0,
        vars: HashMap::new(),
        exit: None,
        reader: Reader::new(stdin.lock()),
    };
    while ctx.pc < ctx.len {
        if let Some(statement) = &program.statements[ctx.pc] {
            ctx.pc += 1;
            let pc = ctx.pc;
            if let Err(e) = run_statement(&mut ctx, statement) {
                eprintln!("{}번째 줄: {}", pc, e);
                ctx.exit = Some(-1);
            }
        } else {
            ctx.pc += 1;
        }
        if ctx.exit.is_some() {
            break;
        }
    }
    ctx.exit.unwrap_or(0)
}

struct Context<'a> {
    len: usize,
    pc: usize,
    vars: HashMap<usize, i32>,
    exit: Option<i32>,
    reader: whiteread::Reader<std::io::StdinLock<'a>>,
}

enum Error {
    GotoOutOfRange,
    UnicodeOutOfRange,
    InputNotNumber,
}

impl Display for Error {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            Self::GotoOutOfRange => write!(f, "\"준\" 명령에 주어진 줄 수가 범위를 벗어났습니다."),
            Self::UnicodeOutOfRange => {
                write!(f, "\"식ㅋ\" 명령에 주어진 유니코드가 범위를 벗어났습니다.")
            }
            Self::InputNotNumber => write!(f, "입력이 수가 아닙니다."),
        }
    }
}

fn run_statement(ctx: &mut Context, statement: &Statement) -> Result<(), Error> {
    match statement {
        Statement::Assign { index, value } => {
            let value = match value {
                Some(multiply) => get_multiply(ctx, multiply)?,
                None => 0,
            };
            ctx.vars
                .entry(*index)
                .and_modify(|v| *v = value)
                .or_insert(value);
        }
        Statement::PrintInt { value } => {
            let value = match value {
                Some(multiply) => get_multiply(ctx, multiply)?,
                None => 0,
            };
            print!("{}", value);
        }
        Statement::PrintChar { codepoint } => {
            let char = match codepoint {
                Some(multiply) => {
                    let code: u32 = get_multiply(ctx, multiply)?
                        .try_into()
                        .map_err(|_| Error::UnicodeOutOfRange)?;
                    char::from_u32(code).ok_or(Error::UnicodeOutOfRange)?
                }
                None => '\n',
            };
            print!("{}", char);
        }
        Statement::If {
            condition,
            statement,
        } => {
            let condition = match condition {
                Some(multiply) => get_multiply(ctx, multiply)?,
                None => 0,
            };
            if condition == 0 {
                return run_statement(ctx, statement.as_ref());
            }
        }
        Statement::Goto { line } => {
            let line = get_multiply(ctx, line)?;
            if 1 <= line && (line as usize) <= ctx.len {
                ctx.pc = line as usize - 1;
            } else {
                return Err(Error::GotoOutOfRange);
            }
        }
        Statement::Exit { code } => {
            let code = match code {
                Some(multiply) => get_multiply(ctx, multiply)?,
                None => 0,
            };
            ctx.exit = Some(code);
        }
    }
    Ok(())
}

fn get_multiply(ctx: &mut Context, multiply: &Multiply) -> Result<i32, Error> {
    let mut result = 1i32;
    for term in &multiply.terms {
        let load = term
            .load
            .as_ref()
            .map(|&Load { index }| *ctx.vars.entry(index).or_insert(0))
            .unwrap_or(0);
        let mut term_value = load.wrapping_add(term.add);
        for _ in 0..term.input {
            let add = ctx.reader.parse().map_err(|_| Error::InputNotNumber)?;
            term_value = term_value.wrapping_add(add);
        }
        let new_result = result.wrapping_mul(term_value);
        result = new_result;
    }
    Ok(result)
}
