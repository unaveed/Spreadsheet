all:
	g++ server.cc globals.cc stringsocket.cc -pthread





stringsocket: stringsocket.o
	g++ stringsocket.o -pthread -o stringsocket

stringsocket.o: stringsocket.cc
	g++ -c stringsocket.cc -pthread

clean:
	+rm *.o stringsocket
