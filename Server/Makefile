all: server.o messages.o
	g++ -o a.out server.o messages.o

server.o: server.cc server.h 
	g++ -c server.cc 

messages.o: messages.cc messages.h
	g++ -c messages.cc

clean:
	rm -rf *.o a.out 
