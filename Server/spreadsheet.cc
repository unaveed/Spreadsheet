#include <set>
#include <map>
#include <stack>
#include <string>
#include <fstream>
#include <iostream>
#include <sstream>

#include "spreadsheet.h"
#include "DependencyGraph/DependencyGraph.h"

using namespace std;

spreadsheet::spreadsheet(char * _filename) {
	clients    = new set<int>;
	cells      = new map<string, string>;
	undo_stack = new stack<string>;
	version    = 1;
	filename   = _filename;

	open();
}

spreadsheet::~spreadsheet() {
	delete clients;
	delete cells;
	delete undo_stack;
}

void spreadsheet::open() {
	string line;
	string name, contents;
	ifstream file(filename);
	if (file.is_open()) {
		getline(file, line);	// <spreadsheet>
		getline(file, line);	// <version>
		getline(file, line);	// Version
		istringstream (line) >> version;	// Version
		getline(file, line);	// </version>
		while (getline(file, line)) {
			if (line == "</spreadsheet>")
				break;
			// <cell>
			getline(file, line);	// <name>
			getline(file, line);	// Name
			name = line;
			getline(file, line);	// </name>
			getline(file, line);	// <contents>
			getline(file, line);	// Contents
			contents = line;
			getline(file, line);	// </contents>
			getline(file, line);	// </cell>

			// Fill the cell
			(*cells)[name] = contents;
		}

		file.close();
	}
	else
		cout << "Unable to open file." << endl;
}

void spreadsheet::save() {
	ofstream file(filename);
	if (file.is_open()) {
		file << "<spreadsheet>\n";
		file << "<version>\n";
		file << version << "\n";
		file << "</version>\n";
		for (map<string, string>::iterator it = cells->begin(); it != cells->end(); ++it) {
			file << "<cell>\n";
			file << "<name>\n" << it->first << "\n" << "</name>\n";
			file << "<contents>\n" << it->second << "\n" << "</contents>\n";
			file << "</cell>\n";
		}
		file << "</spreadsheet>";
		file.close();
	}
	else
		cout << "Unable to open file." << endl;
}

void spreadsheet::make_change(std::string) {
	version++;
}

void spreadsheet::add_client(int client) {
	clients->insert(client);
}

void spreadsheet::remove_client(int client) {
	clients->erase(client);
}

int spreadsheet::get_version() {
	return version;
}

void spreadsheet::undo() {
	// TODO: Implement
}
