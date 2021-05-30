#ifndef STRING_H
#define STRING_H

#include <string.h>

size_t stridx(const char *_Str1, const char *_Str2);
int strstt(const char *_Str1, const char *_Str2);
void strsub(char *_Des, const char *_Str, size_t _Idx, size_t _Len);

#endif