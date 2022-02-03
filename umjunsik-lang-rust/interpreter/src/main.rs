use std::{collections::HashMap, fmt::Display, io::Write};

use clap::Parser;
use umjunsik::{
    ast::{Int, Load, PureInt, Statement},
    parser::parse_statement,
};

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
        interpret(&code)
    } else {
        repl().expect("REPL 실행 중 오류가 발생했습니다.")
    };
    std::process::exit(exit_code);
}

fn repl() -> std::io::Result<i32> {
    let mut ctx = Context::default();
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

    let mut ctx = Context {
        len: program.0.len(),
        ..Default::default()
    };
    while ctx.pc < ctx.len {
        if let Some(statement) = &program.0[ctx.pc] {
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

#[derive(Default)]
struct Context {
    len: usize,
    pc: usize,
    vars: HashMap<usize, i32>,
    exit: Option<i32>,
}

enum Error {
    Overflow,
    GotoOutOfRange,
    UnicodeOutOfRange,
}

impl Display for Error {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            Self::Overflow => write!(f, "계산 도중 오버플로가 일어났습니다."),
            Self::GotoOutOfRange => write!(f, "\"준\" 명령에 주어진 줄 수가 범위를 벗어났습니다."),
            Self::UnicodeOutOfRange => {
                write!(f, "\"식ㅋ\" 명령에 주어진 유니코드가 범위를 벗어났습니다.")
            }
        }
    }
}

fn run_statement(ctx: &mut Context, statement: &Statement) -> Result<(), Error> {
    match statement {
        Statement::Assign(i, num) => {
            let num = match num {
                Some(Int::Pure(pure_int)) => get_pure_int(ctx, pure_int)?,
                Some(Int::IO) => read_int(),
                None => 0,
            };
            ctx.vars.entry(*i).and_modify(|v| *v = num).or_insert(num);
        }
        Statement::PrintInt(num) => {
            let num = match num {
                Some(pure_int) => get_pure_int(ctx, pure_int)?,
                None => 0,
            };
            print!("{}", num);
        }
        Statement::PrintChar(num) => {
            let char = match num {
                Some(pure_int) => {
                    let code: u32 = get_pure_int(ctx, pure_int)?
                        .try_into()
                        .map_err(|_| Error::UnicodeOutOfRange)?;
                    char::from_u32(code).ok_or(Error::UnicodeOutOfRange)?
                }
                None => '\n',
            };
            print!("{}", char);
        }
        Statement::If(num, statement) => {
            let num = match num {
                Some(pure_int) => get_pure_int(ctx, pure_int)?,
                None => 0,
            };
            if num == 0 {
                return run_statement(ctx, statement.as_ref());
            }
        }
        Statement::Goto(num) => {
            let num = match num {
                Some(pure_int) => get_pure_int(ctx, pure_int)?,
                None => 0,
            };
            if 1 <= num && (num as usize) <= ctx.len {
                ctx.pc = num as usize - 1;
            } else {
                return Err(Error::GotoOutOfRange);
            }
        }
        Statement::Exit(num) => {
            let num = match num {
                Some(pure_int) => get_pure_int(ctx, pure_int)?,
                None => 0,
            };
            ctx.exit = Some(num);
        }
    }
    Ok(())
}

fn get_pure_int(ctx: &mut Context, pure_int: &PureInt) -> Result<i32, Error> {
    let mut result = 1i32;
    for term in &pure_int.0 {
        let load = term
            .0
            .as_ref()
            .map(|&Load(i)| *ctx.vars.entry(i).or_insert(0))
            .unwrap_or(0);
        let (term_value, overflow) = load.overflowing_add(term.1);
        if overflow {
            return Err(Error::Overflow);
        }
        let (new_result, overflow) = result.overflowing_mul(term_value);
        if overflow {
            return Err(Error::Overflow);
        }
        result = new_result;
    }
    Ok(result)
}

fn read_int() -> i32 {
    let mut line = String::new();
    if std::io::stdin().read_line(&mut line).is_err() {
        0
    } else {
        line.parse().unwrap_or(0)
    }
}
