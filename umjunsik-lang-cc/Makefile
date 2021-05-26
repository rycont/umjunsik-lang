C_SOURCE = $(wildcard src/*.c)
HEADER = $(wildcard src/*.h)
OBJ = $(C_SOURCE:.c=.o)

CFLAGS = -Wall

CC = gcc

ummc: ${OBJ}
	${CC} ${CFLAGS} $^ -o $@

%.o: %.c ${HEADER}
	${CC} ${CFLAGS} -c $< -o $@

clean:
	rm -rf ${OBJ} a.out ummc