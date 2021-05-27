#include <stdio.h>
#include <stdlib.h>
#include <wchar.h>
#include "info.h"
#include "parse.h"
#include "string.h"
#include "file.h"

int main(int argc, char *argv[]) {
    FILE *input = NULL;
    FILE *temp = NULL;
    int mode = 0;
    char input_path[4096] = {0};
    char temp_path[4096] = {0};
    char output_path[4096] = {0};
    char command[8500] = {0};
    char cflags[100] = {' ', 0};
    int i;
    for(i=1; i<argc; i++) {
        if(argv[i][0]=='-') {
            if(strcmp(argv[i], "-h")==0 || strcmp(argv[i], "--help")==0) {
                fprintf(stdout, HELP_S);
                exit(0);
            } else if(strcmp(argv[i], "-v")==0 || strcmp(argv[i], "--version")==0) {
                fprintf(stdout, VERSION_S);
                exit(0);
            } else if(strcmp(argv[i], "-o")==0 && i + 1 < argc) {
                strcpy(output_path, argv[++i]);
                strcat(cflags, "-o ");
                strcat(cflags, argv[i]);
                strcat(cflags, " ");
            } else if(strcmp(argv[i], "-c")==0) {
                mode |= SRC;
            } else if(strcmp(argv[i], "-s")==0) {
                strcat(cflags, "-S ");
            } else {
                fprintf(stderr, "어떻게 이게 옵션이냐ㅋㅋ\n");
                exit(1);
            }
        } else {
            strcpy(input_path, argv[i]);
        }
    }
    if(input_path[0] == 0) {
        fprintf(stderr, "어떻게 파일 이름이 없냐ㅋㅋ\n");
        exit(1);
    }
    if(strcmp(strrchr(input_path, '.'), ".umm")) {
        fprintf(stderr, "어떻게 %s가 umm파일이냐ㅋㅋ\n", strrchr(input_path, '.'));
        exit(1);
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
    if(mode & SRC) {
        if(output_path[0]) {
            moveFile(temp_path, output_path);
        }
    } else {
        sprintf(command, "gcc%s\"%s\"", cflags, temp_path);
        system(command);
        remove(temp_path);
    }
    return 0;
}