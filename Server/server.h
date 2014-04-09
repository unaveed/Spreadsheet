#ifndef SERVER_H
#define SERVER_H
#include <iostream>
#include <string>
#include <stack>
#include "messages.h"

class Server {
	private:
		std::stack<std::string> undoStack;
		int port;
	public:
		Server(int);
		void undo();

		/***** For debugging only *****/
		void push_edit(std::string);
};
#endif


