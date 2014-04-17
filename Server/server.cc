/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#include <iostream>
#include <set>
#include "server.h"
#include <dirent.h>
#include "messages.h"
#include <string>
#include <cstring>

using namespace std;

int main() {
	server s;
	s.run_server();
}

server::server() {
	password = "12345";
	clients = new set<int>;
	spreadsheets = new map<string, spreadsheet* >;
	clientSpreadsheets = new map<int, string>;
	m = new Messages();
}

server::~server() {
	delete clients;
	delete spreadsheets;
	delete clientSpreadsheets;
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
void server::message_received(int client, string input) {
	cout << input << endl;
	string command, message;

	m->receive_message(input, command, message);

	string filelist;

	// Handle authentication
	if(command == "PASSWORD"){
		// Create spreadsheet and send it's
		// contents to the client
		if(message == password){
			filelist = "FILELIST\e";
			filelist.append( get_files() );
		
			// Send list of files to the client
			send_message_client(filelist, client);
			
			// Add client to the list of clients
			add_client(client);
		}
		// Send invalid password to socket
		else 
			send_message_client("INVALID\n", client);	
	}
	else if(command == "CREATE") {
		filelist = get_files();

		// File with the given name already exists
		if(filelist.find(message) >= 0) 
			send_message_client("ERROR\eFile already exists\n", client);
		
		else {
			// Create a new spreadsheet, add client to spreadsheet
			// and add it to the list of spreadsheets
			char *cstr = new char[message.length() + 1];
			strcpy(cstr, message.c_str());
		
			spreadsheet *ss = new spreadsheet(cstr);
			spreadsheets->insert(pair<string, spreadsheet*> (message, ss) );
			clientSpreadsheets->insert(pair<int, string> (client, message) );
		}
	}
	else if(command == "OPEN") {
		filelist = get_files();
		// Check if file exists
		if(filelist.find(message) >= 0) {
			// Create a spreadsheet and add spreadsheet to the
			// map and client to the spreadsheet
			char *cstr = new char[message.length() + 1];
			strcpy(cstr, message.c_str());
			
			spreadsheet *ss = new spreadsheet(cstr);
			spreadsheets->insert(pair<string, spreadsheet*> (message, ss) );
			clientSpreadsheets->insert(pair<int, string> (client, message) );
		}
		// Send error message to client requesting file that does exist
		else 
			send_message_client("ERROR\eFile does not exist\n", client);
	}
	else 
		execute_command(client, command, message);
}

/*
 * Retrieves the available spreadsheet files from
 * the current directory and returns them as a string
 */
string server::get_files() {
	string files = "";
	DIR *d;
	struct dirent *dir;
	// Flag that allows escape characters can be added after first spreadsheet file 
	int addDelimiter = 0;

	// Look in the current directory for files
	d = opendir(".");

	if(d) {
		// Holds the name of the file
		string fileName;
		
		while ((dir = readdir(d)) != NULL) {
			fileName = string(dir->d_name);	
		
			// Check for file names that are large enough to be spreadsheet files
			if(fileName.length() > 3) {
				// Get the file extension
				string segment = fileName.substr(fileName.length() - 3);
				if(segment == ".ss") {
					// If first spreadsheet file added, add escape characters
					if(addDelimiter > 0)  {
						files.append("\e");
						files.append(fileName);
					}
					else {
						files.append(fileName);
					}
					addDelimiter++;
				}
			}	
		}
		closedir(d);
		files.append("\n");
	}
	return files; 
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

/*
 * Based on command, finds the spreadsheet that the
 * client belongs to and calls the appropriate method
 * based on the command. 
 */
void server::execute_command(int client, string command, string message) {
	string sheet = get_spreadsheet(client);
	spreadsheet *s = (*spreadsheets)[sheet];
	
	if(command == "ENTER") 
		s->make_change(message);
	if(command == "UNDO") 
		s->undo();	
	if(command == "SAVE") 
		s->save();	
	if(command == "RESYNC") {
		// TODO: resync with spreadsheet
	}
	(*spreadsheets)[sheet] = s;
}

/* 
 * Finds which spreadsheet a client belongs to.
 */
string server::get_spreadsheet(int client) {
	typedef map<int, string>::iterator it_type;
	for(it_type it = clientSpreadsheets->begin(); it != clientSpreadsheets->end(); it++) {
		if(it->first == client)
			return it->second; 
	}
	return "";
}
