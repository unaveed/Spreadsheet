#include "server.h"
#include "messages.h"
#include <boost/lexical_cast.hpp>

/* TODO: Questions 
 * How will different spreadsheets have different stacks?
 *  - How will this be initialized
 */
Server::Server(int port){
	this->port = port;
	this->version = 0;
}

void Server::undo(){
	// Check for non-empty stack
	if(!undoStack.empty()){
		std::string edit = undoStack.top();
		undoStack.pop();
		Messages *mess = new Messages("ENTER", edit);
		mess->send_message();
	}
}

void Server::push_edit(std::string edit){
	undoStack.push(edit);
}

int main(int argc, char* argv[]) {
	if (argc == 2){
		int port = boost::lexical_cast<int>(argv[1]);
		Server *u = new Server(port);
	}
	else {
		std::cout << "Incorrect number of arguments, please provide port number.\n" << std::endl;
		return 1;
	}

	return 0;
}
