/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#include <iostream>
#include <set>

#include "server.h"

using namespace std;


int main() {
	server s;
	s.run_server();
}

server::server() {
	clients = new set<int>;
}

server::~server() {
	delete clients;
}

void server::run_server() {
	start(*this);
}

/*
 * Sends a message to all clients of a specific spreadsheet.
 */
void server::send_message_all(string message) {
	for (set<int>::iterator it = clients->begin(); it != clients->end(); ++it)
		server_send(*it, message);
}

/*
 * Sends a message to a specific client.
 */
void server::send_message_client(string message, int client) {
	server_send(client, message);
}

/*
 * Called when a message has been received from a client.
 */
void server::message_received(int client, string message) {
	cout << message << endl;
	send_message_all(message);
	send_message_client(message, client);
}

/*
 * Adds a client to the set of clients.
 */
void server::add_client(int client) {
	clients->insert(client);
}

/*
 * Removes a client from the set of clients.
 */
void server::remove_client(int client) {
	clients->erase(client);
}
