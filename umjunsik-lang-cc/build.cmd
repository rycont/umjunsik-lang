@echo off
gcc -Wall -c src\file.c -o src\file.o
gcc -Wall -c src\main.c -o src\main.o
gcc -Wall -c src\parse.c -o src\parse.o
gcc -Wall -c src\string.c -o src\string.o
gcc -Wall src\file.o src\main.o src\parse.o src\string.o -o ummc.exe