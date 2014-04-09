all: server.o messages.o
	g++ -o a.out server.o messages.o

server.o: server.cpp server.h 
	g++ -c server.cpp 

messages.o: messages.cpp messages.h
	g++ -c messages.cpp

clean:
	rm -rf *.o a.out 
