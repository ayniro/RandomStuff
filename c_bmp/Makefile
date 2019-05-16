CFLAGS=-Iinclude -g -std=c99 -Wall -Werror

DIR = bin
build_folder := $(shell mkdir -p $(DIR))

main: bin/main.o bin/bmp.o
	gcc $(CFLAGS) bin/main.o bin/bmp.o -o main

bin/main.o: src/main.c include/bmp.h
	gcc $(CFLAGS) -c src/main.c -o bin/main.o

bin/bmp.o: src/bmp.c include/bmp.h
	gcc $(CFLAGS) -c src/bmp.c -o bin/bmp.o

clean:
	rm -f bin/* main
	rmdir bin

