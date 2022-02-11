# umjunsik-lang-rust

엄랭의 Rust 구현체입니다. 현재 파서(umjunsik, `/lib`)와 인터프리터(rummi, `/interpreter`),
컴파일러(rummc, `/compiler`)가 개발되어 있습니다.

## 인터프리터

`cargo run -p rummi`로 REPL을, `cargo run -p rummi -- <파일명>`으로 코드 파일을 실행할 수 있습니다.

## 컴파일러

`cargo run -p rummc -- <파일명> -o <출력 파일명>`로 object 파일을 생성할 수 있습니다.

이후 `gcc -o <실행 파일명> <object 파일명>`이나
`clang -o <실행 파일명> <object 파일명>`으로 실행 파일을 생성할 수 있습니다.
