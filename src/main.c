#include <stdio.h>
#include <stdlib.h>
#include <wchar.h>
#ifdef __unix__
#include <unistd.h>
#endif
#include "info.h"
#include "parse.h"
#include "string.h"
#include "file.h"

int main(int argc, char *argv[])
{
    FILE *input = NULL;
    FILE *output = NULL;
    int mode = 0;
    char input_path[4096] = {0};
    char output_path[4096] = {0};
    char command[8500] = {0};
    int i;
    for (i = 1; i < argc; i++)
    {
        if (argv[i][0] == '-')
        {
            if (strcmp(argv[i], "-h") == 0 || strcmp(argv[i], "--help") == 0)
            {
                fprintf(stdout, HELP_S);
                exit(0);
            }
            else if (strcmp(argv[i], "-v") == 0 || strcmp(argv[i], "--version") == 0)
            {
                fprintf(stdout, VERSION_S);
                exit(0);
            }
            else if (strcmp(argv[i], "-o") == 0 && i + 1 < argc)
            {
                strcpy(output_path, argv[++i]);
            }
            else if (strcmp(argv[i], "-c") == 0)
            {
                mode = SRC;
            }
            else if (strcmp(argv[i], "-s") == 0)
            {
                mode = ASM;
            }
            else
            {
                fprintf(stderr, "어떻게 이게 옵션이냐ㅋㅋ\n");
                exit(1);
            }
        }
        else
        {
            strcpy(input_path, argv[i]);
        }
    }
    if (input_path[0] == 0)
    {
        fprintf(stderr, "어떻게 파일 이름이 없냐ㅋㅋ\n");
        exit(1);
    }
    if (strcmp(strrchr(input_path, '.'), ".umm"))
    {
        fprintf(stderr, "어떻게 %s가 umm파일이냐ㅋㅋ\n", strrchr(input_path, '.'));
        exit(1);
    }
    input = fopen(input_path, "r");
    if (input == NULL)
    {
        fprintf(stderr, "어떻게 %s가 파일 이름이냐ㅋㅋ\n", input_path);
        exit(1);
    }
#ifdef __unix__
    fseek(input, 0L, SEEK_END);
    long l = ftell(input);
    rewind(input);
    char *text = malloc(sizeof(char) * l + 1);
    memset(text, 0, sizeof(char) * l + 1);
    fread(text, sizeof(char), l, input);
    fclose(input);
    int ispipe = 1;

    switch (mode)
    {
    case SRC:
        if (output_path[0] == 0)
        {
            strcpy(output_path, strrchr(input_path, '/') + 1);
            strcpy(strrchr(output_path, '.'), ".c");
        }
        output = fopen(output_path, "w");
        ispipe = 0;
        break;
    case ASM:
        if (output_path[0] == 0)
        {
            strcpy(output_path, strrchr(input_path, '/') + 1);
            strcpy(strrchr(output_path, '.'), ".s");
        }
        sprintf(command, "gcc -S -x c - -o \"%s\"", output_path);
        output = popen(command, "w");
        break;
    default:
        if (output_path[0] == 0)
        {
            strcpy(output_path, "a.out");
        }
        sprintf(command, "gcc -x c - -o \"%s\"", output_path);
        output = popen(command, "w");
        break;
    }
    if (parse(text, output))
    {
        fprintf(stderr, "어떻게 이 코드가 엄랭이냐ㅋㅋ\n");
        free(text);
        ispipe ? pclose(output) : fclose(output);
        remove(output_path);
        exit(1);
    }
    free(text);
    ispipe ? pclose(output) : fclose(output);
#else
    char temp_path[4096] = {0};
    strcpy(temp_path, strrchr(input_path, '/') + 1);
    strcpy(strrchr(temp_path, '.'), ".c");
    fseek(input, 0L, SEEK_END);
    long l = ftell(input);
    rewind(input);
    char *text = malloc(sizeof(char) * l + 1);
    memset(text, 0, sizeof(char) * l + 1);
    fread(text, sizeof(char), l, input);
    fclose(input);
    output = fopen(temp_path, "w");
    if (parse(text, output))
    {
        fprintf(stderr, "어떻게 이 코드가 엄랭이냐ㅋㅋ\n");
        free(text);
        fclose(output);
        remove(temp_path);
        exit(1);
    }
    free(text);
    fclose(output);
    switch (mode)
    {
    case SRC:
        if (output_path[0])
        {
            moveFile(temp_path, output_path);
            remove(temp_path);
        }
        break;
    case ASM:
        if (output_path[0] == 0)
        {
            strcpy(output_path, strrchr(input_path, '/') + 1);
            strcpy(strrchr(output_path, '.'), ".s");
        }
        sprintf(command, "gcc -S \"%s\" -o \"%s\"", temp_path, output_path);
        system(command);
        remove(temp_path);
        break;
    default:
        if (output_path[0] == 0)
        {
            sprintf(command, "gcc \"%s\"", temp_path);
        }
        else
        {
            sprintf(command, "gcc \"%s\" -o \"%s\"", temp_path, output_path);
        }
        system(command);
        remove(temp_path);
        break;
    }
#endif
    return 0;
}