#include "file.h"
#include <stdio.h>

void moveFile(char *_Old, char *_New) {
    FILE *old = fopen(_Old, "r");
    FILE *new = fopen(_New, "w");

    char c;
    while((c = fgetc(old)) != EOF) {
        fputc(c, new);
    }

    fclose(old);
    fclose(new);

    remove(_Old);
}