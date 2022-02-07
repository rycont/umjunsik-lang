C_SOURCES = $(wildcard src/*.c)
HEADERS = $(wildcard src/*.h)
OBJ = $(C_SOURCES:src/%.c=bin/%.o)

CC = gcc
CFLAGS = -Wall

TARGET = bin/umcc

${TARGET}: ${OBJ}
	${CC} ${CFLAGS} $^ -o $@

bin/%.o: src/%.c ${HEADERS} bin
	${CC} ${CFLAGS} -c $< -o $@

bin:
	mkdir bin

clean:
	rm -rf ${OBJ} ${TARGET} bin