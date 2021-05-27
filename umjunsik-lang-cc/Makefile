C_SOURCES = $(wildcard src/*.c)
HEADERS = $(wildcard src/*.h)
OBJ = $(C_SOURCES:.c=.o)

CC = gcc
CFLAGS = -Wall

ummc: ${OBJ}
	${CC} ${CFLAGS} $^ -o $@

%.o: %.c ${HEADERS}
	${CC} ${CFLAGS} -c $< -o $@

clean:
	rm -rf ${OBJ} a.out ummc