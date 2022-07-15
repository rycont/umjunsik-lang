#include "string.h"

size_t stridx(const char *_Str, const char *_Target) {
    size_t l = strlen(_Target);
    size_t i, j;
    int flag;
    for(i=0; _Str[i]; i++) {
        flag = 1;
        for(j=0; j<l; j++) {
            if(_Str[i+j]!=_Target[j]) {
                flag = 0;
                break;
            }
        }
        if(flag) {
            return i;
        }
    }
    return -1;
}

int strstt(const char *_Str, const char *_Target) {
    size_t l = strlen(_Target);
    size_t i;
    for(i=0; i<l; i++) {
        if(_Str[i]!=_Target[i]) {
            return 0;
        }
    }
    return 1;
}

void strsub(char *_Des, const char *_Str, size_t _Idx, size_t _Len) {
    size_t i;
    for(i=0; i<_Len; i++) {
        _Des[i] = _Str[_Idx+i];
    }
    _Des[_Len] = 0;
    return;
}