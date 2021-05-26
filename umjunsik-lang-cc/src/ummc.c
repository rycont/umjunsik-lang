#include <stdio.h>
#include <stdlib.h>
#include <wchar.h>
#include <unistd.h>
#include "info.h"
#include "parse.h"
#include "string.h"

int main(int argc, char *argv[]) {
    FILE *input = NULL;
    FILE *temp = NULL;
    int mode = 0;
    char input_path[4096];
    char temp_path[4096];
    char output_path[4096];
    char command[8500];
    int opt;
    strcpy(output_path, "a.out");
    while((opt = getopt(argc, argv, "vho:t")) != -1) {
        switch(opt) {
        case 'h':
            mode |= HELP_M;
            break;
        case 'v':
            mode |= VERSION_M;
            break;
        case 'o':
            strcpy(output_path, optarg);
            break;
        case 't':
            mode |= TEMP_M;
            break;
        }
    }
    if(mode & HELP_M) {
        fprintf(stdout, HELP_S);
        exit(0);
    }
    if(mode & VERSION_M) {
        fprintf(stdout, VERSION_S);
        exit(0);
    }
    if(optind >= argc) {
        fprintf(stderr, "어떻게 파일 이름이 없냐ㅋㅋ\n");
        exit(1);
    }
    strcpy(input_path, argv[optind]);
    if(strcmp(strrchr(input_path, '.'), ".umm")) {
        fprintf(stderr, "어떻게 %s가 umm파일이냐ㅋㅋ\n", strrchr(input_path, '.'));
    }
    input = fopen(input_path, "r");
    if(input == NULL) {
        fprintf(stderr, "어떻게 %s가 파일 이름이냐ㅋㅋ\n", input_path);
        exit(1);
    }
    strcpy(temp_path, input_path);
    *strrchr(temp_path, '.') = 0;
    strcat(temp_path, ".c");
    fseek(input, 0L, SEEK_END);
    long l = ftell(input);
    rewind(input);
    char *text = malloc(sizeof(char)*l+1);
    memset(text, 0, sizeof(char)*l+1);
    fread(text, sizeof(char), l, input);
    fclose(input);
    temp = fopen(temp_path, "w");
    if(parse(text, temp)) {
        fprintf(stderr, "어떻게 이 코드가 엄랭이냐ㅋㅋ\n");
        free(text);
        fclose(temp);
        remove(temp_path);
        exit(1);
    }
    free(text);
    fclose(temp);
    if(mode & TEMP_M) {
        sprintf(command, "gcc -save-temps \"%s\" -o \"%s\"", temp_path, output_path);
        system(command);
    } else {
        sprintf(command, "gcc \"%s\" -o \"%s\"", temp_path, output_path);
        system(command);
        remove(temp_path);
    }
    return 0;
}