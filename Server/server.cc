/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#include <iostream>
#include <set>
#include <dirent.h>
#include <string>
#include <cstring>
#include <pthread.h>

#include "server.h"
#include "messages.h"
#include "spreadsheet.h"

using namespace std;

void *StartServer(void *arguments) {
	string d = *((string *) &arguments);

	string input;
	getline(cin, input);	

	if(input == "exit") {
		cout << "It worked" << endl;
		pthread_exit(NULL);
	}
	else { 
		cout << "You entered: " << input << endl;	
		StartServer(&arguments);
	}

}


int main() {
	server s;	
//	pthread_t threads[1];
	
	// Start a new thread to listen for input
	//int rc = pthread_create(&threads[0], NULL, StartServer, (void *) &check);
	//pthread_create(&threads[0], NULL, StartStuff, (void *) &s);
	
	s.run_server();
	

	// Close thread
//	pthread_exit(NULL);
}

server::server() {
	password  = "12345";
	path      = "files/";
	delimiter = "\e";
	clients   = new set<int>;
	spreadsheets       = new map<string, spreadsheet* >;
	clientSpreadsheets = new map<int, string>;
}

void server::run_server() {
	start(*this);
}

server::~server() {
	delete clients;
	delete spreadsheets;
	delete clientSpreadsheets;
}

/*
 * Sends a message to a specific client.
 */
void server::send_message_client(string message, int client) {
	server_send(client, message);
}

void server::send_message(set<int> & client, string message) {
	for (set<int>::iterator it = client.begin(); it != client.end(); ++it)
		server_send(*it, message);
}

/*
 * Called when a message has been received from a client.
 */
void server::message_received(int client, string input) {
	string command, message;
	
	Messages *m = new Messages(*this);
	m->receive_message(input, command, message);

	string filelist;

	// Handle authentication
	if(command == "PASSWORD"){
		// Create spreadsheet and send it's
		// contents to the client
		if(message == password){
			cout << "Client " << client << " authenticated." << endl;
			filelist = "FILELIST";
			filelist.append(delimiter);
			filelist.append( get_files(true) );
		
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
		filelist = get_files(false);
		
		size_t found = filelist.find(message);
		// File with the given name already exists
		if(found != std::string::npos) 
			send_message_client("ERROR\eFile already exists\n", client);
		
		else {
			// Create a new spreadsheet, add client to spreadsheet
			// and add it to the list of spreadsheets

			// GREG
			message.insert(0, path);
			// END GREG

			const char * cstr = message.c_str(); 
			
			spreadsheet *ss = new spreadsheet(cstr, new Messages(*this), false);
			ss->add_client(client);
			spreadsheets->insert(pair<string, spreadsheet*> (message, ss) );
			clientSpreadsheets->insert(pair<int, string> (client, message) );
		}
	}
	else if(command == "OPEN") {
		filelist = get_files(true);
		string sheetFiles = get_files(false);
		// Check if file exists
		if(filelist.find(message) >= 0) {
			// Create a spreadsheet and add spreadsheet to the
			// map and client to the spreadsheet
			const char * cstr = message.c_str(); 
			
			spreadsheet *ss = new spreadsheet(cstr, new Messages(*this), true);
			ss->add_client(client);
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
 * the current directory and returns them as a string.
 * Clean determines whether only the file name will be
 * saved to the string (when true) or the file name with
 * the path will be saved to the string (when false).
 */
string server::get_files(bool clean) {
	string folder = "files/";
	string files = "";
	DIR *d;
	struct dirent *dir;
	// Flag that allows escape characters can be added after first spreadsheet file 
	int addDelimiter = 0;

	// Look in the directory names 'files' for files
	d = opendir("files/");

	if(d) {
		// Holds the name of the file
		string fileName;
		
		while ((dir = readdir(d)) != NULL) {
			// Adds path in front of file name
			if(!clean){
				fileName = folder;
				fileName.append(string(dir->d_name));
			}
			// Only contains the file name
			else
				fileName = string(dir->d_name);	
		
			// Check for file names that are large enough to be spreadsheet files
			if(fileName.length() > 3) {
				// Get the file extension
				string segment = fileName.substr(fileName.length() - 3);
				if(segment == ".ss") {
					// If first spreadsheet file added, add escape characters
					if(addDelimiter > 0)  {
						files.append(delimiter);
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
		cout << files << endl;
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
	Messages *m = new Messages(*this);	
	string sheet = get_spreadsheet(client);
	spreadsheet *s = (*spreadsheets)[sheet];
	
	if(command == "ENTER") {
		string cellName, contents, version;
		m->split_edit(message, version, cellName, contents);
		s->make_change(client, cellName, contents, version);
	}
	if(command == "UNDO") {
		s->undo();	
	}
	if(command == "SAVE") {
		s->save();	
	}
	if(command == "RESYNC") {
		s->sync(client);	
	}
	(*spreadsheets)[sheet] = s;
}

/*
 * Calls on every spreadsheet in the 
 * spreadsheets map to save their files
 */
void server::save_spreadsheets() {
	map<string, spreadsheet*>::iterator it;
	for(it = spreadsheets->begin(); it != spreadsheets->end(); ++it){
		// Calls the save method for every spreadsheet
		// it->second.save();				
	}
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
