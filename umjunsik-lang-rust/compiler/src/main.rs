use std::{cell::RefCell, collections::HashMap, path::Path};

use clap::Parser;
use guess_host_triple::guess_host_triple;
use inkwell::{
    basic_block::BasicBlock,
    builder::Builder,
    context::Context,
    module::{Linkage, Module},
    targets::{CodeModel, FileType, RelocMode, Target, TargetTriple},
    values::{FunctionValue, IntValue, PointerValue},
    AddressSpace, IntPredicate, OptimizationLevel,
};
use umjunsik::ast::{Load, Multiply, Program, Statement, Term};

/// Umjunsik-lang LLVM Compiler
#[derive(Parser, Debug)]
#[clap(author, version, about)]
struct Args {
    /// Name of source file
    input: String,
    /// Output file name
    #[clap(long, short)]
    output: String,
    #[clap(long, short)]
    /// Compile target triple
    target: Option<String>,
    /// Target features
    #[clap(long, short, default_value = "")]
    features: String,
    /// Optimize aggressively if set
    #[clap(long, short = 'O')]
    optimize: bool,
}

fn main() {
    let args = Args::parse();
    let path = Path::new(&args.input);

    let code = std::fs::read_to_string(&args.input).expect("파일을 읽는 도중 오류가 발생했습니다.");

    let (_, program) =
        umjunsik::parser::parse_program(code.trim()).expect("코드의 문법이 맞지 않습니다.");

    let context = Context::create();
    let module = context.create_module(path.file_name().unwrap().to_str().unwrap());
    Target::initialize_all(&Default::default());
    let host_triple = args
        .target
        .as_deref()
        .or_else(|| guess_host_triple())
        .expect("빌드 타겟을 직접 지정해주세요.");
    let triple = TargetTriple::create(&host_triple);
    let target = Target::from_triple(&triple).expect("지원하지 않는 타겟입니다.");
    let level = if args.optimize {
        OptimizationLevel::Aggressive
    } else {
        OptimizationLevel::None
    };
    let machine = target
        .create_target_machine(
            &triple,
            "",
            &args.features,
            level,
            RelocMode::Default,
            CodeModel::Default,
        )
        .expect("지원하지 않는 타겟입니다.");
    let builder = context.create_builder();
    let main_func = module.add_function(
        "main",
        context.i32_type().fn_type(&[], false),
        Some(Linkage::External),
    );
    let entry = context.append_basic_block(main_func, "entry");
    let compiler = Compiler {
        context: &context,
        builder: &builder,
        module: &module,
        main_func,
        main_block: entry,
        vars: HashMap::new().into(),
    };
    compiler.compile_program(&program);

    compiler
        .module
        .verify()
        .expect("컴파일 오류가 발생했습니다.");

    machine
        .write_to_file(&module, FileType::Object, args.output.as_ref())
        .expect("출력 중 오류가 발생했습니다.");
}

struct Compiler<'ctx, 'a> {
    context: &'ctx Context,
    builder: &'a Builder<'ctx>,
    module: &'a Module<'ctx>,
    main_func: FunctionValue<'ctx>,
    main_block: BasicBlock<'ctx>,
    vars: RefCell<HashMap<usize, PointerValue<'ctx>>>,
}

impl Compiler<'_, '_> {
    fn compile_print_int(&self) {
        let i32_type = self.context.i32_type();
        let printf_type = i32_type.fn_type(
            &[self
                .context
                .i8_type()
                .ptr_type(AddressSpace::Generic)
                .into()],
            true,
        );
        let printf = self
            .module
            .add_function("printf", printf_type, Some(Linkage::External));

        let func_type = self.context.void_type().fn_type(&[i32_type.into()], false);
        let func = self.module.add_function("print_int", func_type, None);
        let entry = self.context.append_basic_block(func, "entry");

        self.builder.position_at_end(entry);
        let format_str = self
            .builder
            .build_global_string_ptr("%d", "format")
            .as_pointer_value();
        self.builder.build_call(
            printf,
            &[format_str.into(), func.get_first_param().unwrap().into()],
            "call_printf",
        );
        self.builder.build_return(None);
    }

    fn compile_print_char(&self) {
        let i32_type = self.context.i32_type();
        let putchar_type = i32_type.fn_type(&[i32_type.into()], false);
        let putchar = self
            .module
            .add_function("putchar", putchar_type, Some(Linkage::External));

        let func_type = self.context.void_type().fn_type(&[i32_type.into()], false);
        let func = self.module.add_function("print_char", func_type, None);
        let entry = self.context.append_basic_block(func, "entry");

        self.builder.position_at_end(entry);
        self.builder.build_call(
            putchar,
            &[func.get_first_param().unwrap().into()],
            "call_putchar",
        );
        self.builder.build_return(None);
    }

    fn compile_read_int(&self) {
        let i32_type = self.context.i32_type();
        let scanf_type = i32_type.fn_type(
            &[self
                .context
                .i8_type()
                .ptr_type(AddressSpace::Generic)
                .into()],
            true,
        );
        let scanf = self
            .module
            .add_function("scanf", scanf_type, Some(Linkage::External));

        let func_type = i32_type.fn_type(&[], false);
        let func = self.module.add_function("read_int", func_type, None);
        let basic_block = self.context.append_basic_block(func, "entry");

        self.builder.position_at_end(basic_block);

        let format = self
            .builder
            .build_global_string_ptr("%d", "format")
            .as_pointer_value();
        let temp = self.builder.build_alloca(i32_type, "temp");
        self.builder
            .build_call(scanf, &[format.into(), temp.into()], "call_scanf");
        let ret = self.builder.build_load(temp, "load_temp").into_int_value();
        self.builder.build_return(Some(&ret));
    }

    fn get_ptr(&self, index: usize) -> PointerValue {
        let current = self.builder.get_insert_block().unwrap();
        self.builder.position_at_end(self.main_block);
        let mut vars = self.vars.borrow_mut();
        let ptr = vars.entry(index).or_insert_with(|| {
            self.builder
                .build_alloca(self.context.i32_type(), &format!("var_{}", index))
        });
        self.builder.position_at_end(current);
        *ptr
    }

    fn compile_load(&self, int: &Load) -> IntValue {
        self.builder
            .build_load(self.get_ptr(int.index), &format!("load_{}", int.index))
            .into_int_value()
    }

    fn compile_term(&self, term: &Term) -> IntValue {
        let i32_type = self.context.i32_type();
        let base = term
            .load
            .as_ref()
            .map(|load| self.compile_load(load))
            .unwrap_or_else(|| i32_type.const_zero());
        let added =
            self.builder
                .build_int_add(base, i32_type.const_int(term.add as u64, true), "add");
        let mut inputs = added;
        let read_int = self.module.get_function("read_int").unwrap();
        for _ in 0..term.input {
            let call = self
                .builder
                .build_call(read_int, &[], "read_int")
                .try_as_basic_value()
                .left()
                .unwrap()
                .into_int_value();
            inputs = self.builder.build_int_add(inputs, call, "add_input");
        }
        inputs
    }

    fn compile_multiply(&self, multiply: &Multiply) -> IntValue {
        let mut prod = self.compile_term(&multiply.terms[0]);
        for term in &multiply.terms[1..] {
            prod = self
                .builder
                .build_int_mul(prod, self.compile_term(term), "mul");
        }
        prod
    }

    fn compile_statement(
        &self,
        next: BasicBlock,
        statement: &Statement,
        blocks: &[BasicBlock],
        addrs: PointerValue,
    ) {
        match statement {
            Statement::Assign { index, value } => {
                let value = value
                    .as_ref()
                    .map(|multiply| self.compile_multiply(multiply))
                    .unwrap_or_else(|| self.context.i32_type().const_zero());
                self.builder.build_store(self.get_ptr(*index), value);
                self.builder.build_unconditional_branch(next);
            }
            Statement::PrintInt { value } => {
                let value = value
                    .as_ref()
                    .map(|multiply| self.compile_multiply(multiply))
                    .unwrap_or_else(|| self.context.i32_type().const_zero());
                let print_int = self.module.get_function("print_int").unwrap();
                self.builder
                    .build_call(print_int, &[value.into()], "call_print_int");
                self.builder.build_unconditional_branch(next);
            }
            Statement::PrintChar { codepoint } => {
                let codepoint = codepoint
                    .as_ref()
                    .map(|multiply| self.compile_multiply(multiply))
                    .unwrap_or_else(|| self.context.i32_type().const_int(10, false));
                let print_char = self.module.get_function("print_char").unwrap();
                self.builder
                    .build_call(print_char, &[codepoint.into()], "call_print_char");
                self.builder.build_unconditional_branch(next);
            }
            Statement::If {
                condition,
                statement,
            } => {
                let condition = condition
                    .as_ref()
                    .map(|multiply| self.compile_multiply(multiply))
                    .unwrap_or_else(|| self.context.i32_type().const_zero());
                let condition = self.builder.build_int_compare(
                    IntPredicate::EQ,
                    condition,
                    self.context.i32_type().const_zero(),
                    "if_condition",
                );
                let then_block = self.context.append_basic_block(self.main_func, "then");
                self.builder
                    .build_conditional_branch(condition, then_block, next);
                self.builder.position_at_end(then_block);
                self.compile_statement(next, statement, blocks, addrs);
            }
            Statement::Goto { line } => {
                let line = self.compile_multiply(line);
                let minus_one = self.context.i32_type().const_int(1, false);
                let line_minus_one = self
                    .builder
                    .build_int_sub(line, minus_one, "goto_minus_one");
                let zero = self.context.i32_type().const_zero();
                let address_ptr = unsafe {
                    self.builder.build_in_bounds_gep(
                        addrs,
                        &[zero, line_minus_one],
                        "index_jump_addr",
                    )
                };
                let address = self.builder.build_load(address_ptr, "fetch_jump_addr");
                self.builder.build_indirect_branch(address, blocks);
            }
            Statement::Exit { code } => {
                let code = code
                    .as_ref()
                    .map(|multiply| self.compile_multiply(multiply))
                    .unwrap_or_else(|| self.context.i32_type().const_zero());
                self.builder.build_return(Some(&code));
            }
        }
    }

    fn compile_program(&self, program: &Program) {
        self.compile_read_int();
        self.compile_print_int();
        self.compile_print_char();

        let mut blocks = vec![];
        let mut addrs = vec![];
        // https://llvm.org/docs/LangRef.html
        // blockaddress always has i8 addrspace(P)* type
        let addr_type = self.context.i8_type().ptr_type(AddressSpace::Generic);
        // safety: addrs must be used only in build_indirect_branch
        for _ in 0..program.statements.len() {
            let block = self.context.append_basic_block(self.main_func, "statement");
            blocks.push(block);
            // panic safety: entry block is self.context.main_block
            addrs.push(unsafe { block.get_address() }.unwrap());
        }
        let addr_array = addr_type.const_array(&addrs);
        let addrs = self.module.add_global(
            addr_type.array_type(addrs.len() as u32),
            None,
            "block_addrs",
        );
        addrs.set_initializer(&addr_array);

        let end = self.context.append_basic_block(self.main_func, "end");
        self.builder.position_at_end(end);
        self.builder
            .build_return(Some(&self.context.i32_type().const_zero()));
        blocks.push(end);

        for (i, (&block, statement)) in blocks.iter().zip(&program.statements).enumerate() {
            self.builder.position_at_end(block);
            if let Some(statement) = statement {
                self.compile_statement(blocks[i + 1], statement, &blocks, addrs.as_pointer_value());
            } else {
                self.builder.build_unconditional_branch(blocks[i + 1]);
            }
        }

        self.builder.position_at_end(self.main_block);
        self.builder.build_unconditional_branch(blocks[0]);
    }
}
