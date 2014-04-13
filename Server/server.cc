/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#include <iostream>
#include <set>
#include "server.h"
#include <dirent.h>

using namespace std;

int main() {
	server s;
	s.message_received(5, "PASSWORD", "124345");
	s.run_server();
}

server::server() {
	password = "abc123";
	clients = new set<int>;
}

server::~server() {
	delete clients;
}

void server::run_server() {
	string j = "dummyText";	
}

/*
 * Sends a message to all clients of a specific spreadsheet.
 */
void server::send_message_all(string message) {
	for (set<int>::iterator it = clients->begin(); it != clients->end(); ++it)
	//	server_send(*it, message);
		string s = "dummyText";
}

/*
 * Sends a message to a specific client.
 */
void server::send_message_client(string message, int client) {
//	server_send(client, message);
}

/*
 * Called when a message has been received from a client.
 */
void server::message_received(int client, string command, string message) {
	cout << message << endl;

	// Check if command has a message, then handle specific commands
	if(message != "") {
		string filelist;
		if(command == "PASSWORD"){
			// Create spreadsheet and send it's
			// contents to the client
			if(message == password){
				filelist = "FILELIST\e";
				filelist.append( get_files() );
				// server_send_client("INVALID\n", client);
			}
			// Send invalid password to socket
			else {
				cout << "danger will robinson" << endl;
				// server_send_client("INVALID\n", client);	
			}
		}
		if(command == "CREATE") {
			filelist = get_files();
			if(filelist.find(message) >= 0) {
				// server_send_client("ERROR\eFile already exists\n", client);
			}
			else {
				// TODO: Create a new spreadsheet, add client to spreadsheet, and add it to the list of spreadsheets
				
				// server_send_client("UPDATE\e1\n");
			}
		}
		if(command == "OPEN") {
			filelist = get_files();
			// Check if file exists
			if(filelist.find(message) >= 0) {
				// TODO: Create a spreadsheet that has a parameter to take in a filename, add
				// spreadsheet to the map, and client to the spreadsheet
			}
			// Send error message to client requesting file that does exist
			else {
				// server_send_client("ERROR\eFile does not exist\n");
			}
		}
	}
	// Handle commands without messages 
	else {

	}
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
