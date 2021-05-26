#ifndef INFO_H
#define INFO_H

#define HELP_S \
"Usage: ummc [options] file...\n" \
"Options:\n" \
"  -h | --help       Display this helpful text :)\n" \
"  -v | --version    Display compiler information.\n" \
"  -o <file>         Place the output into <file>.\n" \
"  -save-temps       Do not delete intermediate files.\n" \
"\n" \
"For bug report, please contact\n" \
"<yangeh2225@gmail.com>.\n"
#define VERSION_S \
"ummc 1.0\n" \
"Copyright (C) 2021 Rokr0k\n" \
"\n" \
"umjunsik-lang\n" \
"Copyright (C) 2020 RyCont\n"

#define HELP_M 0x1
#define VERSION_M 0x2
#define TEMP_M 0x4

#endif