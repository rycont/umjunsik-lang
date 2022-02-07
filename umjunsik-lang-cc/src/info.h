#ifndef INFO_H
#define INFO_H

#define HELP_S \
"Usage: umcc [options] file...\n" \
"Options:\n" \
"  -h | --help       Display this helpful text :)\n" \
"  -v | --version    Display compiler information.\n" \
"  -o <file>         Place the output into <file>.\n" \
"  -c                Compile to C source file.\n" \
"  -s                Compile to assembly source file.\n" \
"\n" \
"For bug report, please contact\n" \
"<yangeh2225@gmail.com>.\n"
#define VERSION_S \
"umcc 1.1\n" \
"Copyright (C) 2022 Rok\n" \
"\n" \
"umjunsik-lang\n" \
"Copyright (C) 2022 Rycont\n"

#define SRC 0x1
#define ASM 0x2

#endif
