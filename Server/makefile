# Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.

CCFILES=server.cc\
		messages.cc\
		spreadsheet.cc\
		stringsocket/globals.cc\
		stringsocket/stringsocket.cc\
		DependencyGraph/DependencyGraph.cc\
		DependencyGraph/CircularDependency.cc


LIBS=	-pthread

TEMP = /usr/local/lib/libboost_*.a -I/usr/local/include/

all:
	g++ -o server $(CCFILES) $(LIBS) $(TEMP)

debug:
	g++ -g -o server $(CCFILES) $(LIBS)

clean:
	rm server
