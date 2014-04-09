#include "server.h"
#include "messages.h"
#include <boost/lexical_cast.hpp>

Server::Server(int port){
	this->port = port;	
}

void Server::undo(){
	// Check for non-empty stack
	if(!undoStack.empty()){
		std::string edit = undoStack.top();
		undoStack.pop();
		Messages *mess = new Messages("ENTER", edit);
		mess->send_message();

		std::cout << edit << std::endl;
		std::cout << "Get command: " << mess->get_command() << std::endl;
		std::cout << "Get content: " << mess->get_content() << std::endl;
	}
	else
		std::cout << "Uh oh, stack empty" << std::endl;
}

void Server::push_edit(std::string edit){
	undoStack.push(edit);
}

// TODO: Accept a command line argument for port
int main(int argc, char* argv[]) {
	if (argc != 2){
		std::cout << "Incorrect number of arguments, please provide port number.\n" << std::endl;
		return 1;
	}
	else {
		int port = boost::lexical_cast<int>(argv[1]);
		Server *u = new Server(port);
		
		u->push_edit("ENTER[esc]A4[esc]A3*3\n");
		u->push_edit("ENTER[esc]B4[esc]3\n");
		u->push_edit("ENTER[esc]A3[esc]12\n");

		u->undo();
		u->undo();
		u->undo();
		u->undo();

		return 0;
	}
}
