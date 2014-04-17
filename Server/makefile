# Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.

CCFILES=server.cc\
		messages.cc\
		spreadsheet.cc\
		stringsocket/globals.cc\
		stringsocket/stringsocket.cc\
		DependencyGraph/DependencyGraph.cc

LIBS=	-pthread

all:
	g++ -o server $(CCFILES) $(LIBS)

clean:
	rm server
