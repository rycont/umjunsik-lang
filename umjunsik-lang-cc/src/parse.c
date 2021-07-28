#include "parse.h"
#include <stdlib.h>
#include <stdint.h>
#include "string.h"

char **splitLines(char *text) {
    size_t nc=1, i, j, k;
    for(i=0; text[i]; i++) {
        if(text[i] == '\n' || text[i] == '~') {
            nc++;
        }
    }
    char **lines = malloc(sizeof(char *)*(nc+1));
    for(i=0, j=0; i<nc; i++) {
        for(k=0; text[j+k]&&text[j+k]!='\n'&&text[j+k]!='\r'&&text[j+k]!='~'; k++);
        lines[i] = malloc(sizeof(char)*k+1);
        memcpy(lines[i], &text[j], sizeof(char)*k);
        lines[i][k] = '\0';
        j = j + k + (text[j+k]=='\r'?2:1);
    }
    lines[nc] = NULL;
    return lines;
}

void freeLines(char **lines) {
    size_t i=0;
    while(lines[i]) {
        free(lines[i++]);
    }
    free(lines);
}

void value(char *text, FILE *output, int tail) {
    if(stridx(text, " ") != -1) {
        size_t tim = stridx(text, " ");
        text[tim] = 0;
        fprintf(output, "(");
        value(text, output, 0);
        fprintf(output, ")*(");
        value(text+tim+1, output, 0);
        fprintf(output, ")");
    } else if(strstt(text, "식?")) {
        fprintf(output, tail?"+input()":"input()");
        value(text + 4, output, 1);
    } else if(strstt(text, "어")) {
        char *a = text + 3;
        int cnt = 0;
        for(; strstt(a, "어"); a += 3, cnt++);
        fprintf(output, tail?"+var[%d]":"var[%d]", cnt);
        value(a, output, 1);
    } else if(strstt(text, ".")  || strstt(text, ",")) {
        int num = 0;
        int i;
        for(i=0; text[i]=='.'||text[i]==','; i++) {
            if(text[i]=='.') {
                num++;
            } else if(text[i]==',') {
                num--;
            }
        }
        fprintf(output, tail?"+%d":"%d", num);
    } else {
        fprintf(output, tail?"+0":"0");
    }
}

void operation(char *text, FILE *output) {
    size_t l = strlen(text);
    if(strstt(text, "동탄") && stridx(text, "?") != -1) {
        size_t sep = stridx(text, "?");
        char *a;
        fprintf(output, "if((");
        a = malloc(sizeof(char)*l);
        strsub(a, text, 6, sep-2);
        value(a, output, 0);
        fprintf(output, ")==0){");
        operation(text+sep+1, output);
        free(a);
        fprintf(output, "}");
    } else if(stridx(text, "엄") != -1) {
        char *a = text;
        int cnt = 0;
        for(; strstt(a, "어"); a += 3, cnt++);
        fprintf(output, "var[%d]=", cnt);
        value(a+3, output, 0);
        fprintf(output, ";");
    } else if(strstt(text, "식")) {
        if(text[l-1] == '!') {
            fprintf(output, "printf(\"%%d\",");
            char *a = malloc(sizeof(char)*l);
            strsub(a, text, 3, l-4);
            value(a, output, 0);
            free(a);
            fprintf(output, ");");
        } else if(strstt(text+l-3, "ㅋ")) {
            if(l == 6) {
                fprintf(output, "printf(\"\\n\");");
            } else {
                fprintf(output, "printf(\"%%c\",");
                char *a = malloc(sizeof(char)*l);
                strsub(a, text, 3, l-6);
                value(a, output, 0);
                free(a);
                fprintf(output, ");");
            }
        }
    } else if(strstt(text, "준")) {
        fprintf(output, "JUN(");
        value(text + 3, output, 0);
        fprintf(output, ")");
    } else if(strstt(text, "화이팅!")) {
        fprintf(output, "exit(");
        value(text + 10, output, 0);
        fprintf(output, ");");
    }
}

int parse(char *text, FILE *output) {
    char **lines = splitLines(text);
    size_t nc;
    size_t i;
    for(nc=0; lines[nc+1]; nc++);
    if(strcmp(lines[0], "어떻게") || strcmp(lines[nc-1], "이 사람이름이냐ㅋㅋ")) {
        freeLines(lines);
        return 1;
    }
    fprintf(output,
    "#include<stdio.h>\n"
    "#include<stdlib.h>\n"
    "#define JUN(n) switch(n){");
    for(i=1; i<nc; i++) {
#ifdef _WIN32
        fprintf(output, "case %u:goto _%u;", i, i);
#else
        fprintf(output, "case %ld:goto _%ld;", i, i);
#endif
    }
    fprintf(output, "}\nint var[65536];void term(){printf(\"\\n\");}int input(){int a;scanf(\"%%d\",&a);return a;}int main(){atexit(term);_1:");
    for(i=1; strcmp(lines[i], "이 사람이름이냐ㅋㅋ"); i++) {
#ifdef _WIN32
        fprintf(output, "_%d:", i+1);
#else
        fprintf(output, "_%ld:", i+1);
#endif
        operation(lines[i], output);
    }
    fprintf(output, "exit(0);}");
    freeLines(lines);
    return 0;
}