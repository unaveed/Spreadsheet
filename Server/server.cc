#include <iostream>

#include "server.h"


extern int start();

int main() {
	server s;
	s.run_server();
	std::cout << "Hello!" << std::endl;
}

server::server() {
	
}

void server::run_server() {
	start();
}

void server::send_message_all(std::string message) {
	// TODO: Implement
	// int server_send(int fd, std::string data);
}

void server::send_message_client(std::string message) {
	// TODO: Implement
}
