#include <set>
#include <map>
#include <stack>
#include <string>
#include <fstream>
#include <iostream>
#include <sstream>
#include <vector>
#include <boost/regex.hpp>

#include "messages.h"
#include "spreadsheet.h"
#include "DependencyGraph/DependencyGraph.h"
#include "DependencyGraph/CircularDependency.h"

using namespace std;

spreadsheet::spreadsheet(string _filename, Messages * _message, bool exists) {
	clients    = new set<int>;
	cells      = new map<string, string>;
	undo_stack_cells    = new stack<string>;
	undo_stack_contents = new stack<string>;
	version    = 0;
	//filename   = _filename;
	message	   = _message;
	dg         = new DependencyGraph();

	filename = _filename;

	if (exists)
		open();
}

spreadsheet::~spreadsheet() {
	delete clients;
	delete cells;
	delete undo_stack_cells;
	delete undo_stack_contents;
}

void spreadsheet::make_change(int client, string name, string contents, string vers) {
	stringstream sv;
	sv << version;
	if (sv.str() != vers) {
		sync(client);
		return;
	}

	// Check if contents is a formula
	if (contents[0] == '=') {
		if (!SetCellContents(name, contents)) {
			message->error(client, contents);
			return;
		}
	}
	else {
		undo_stack_cells->push(name);
		undo_stack_contents->push((*cells)[name]);
		(*cells)[name] = contents;
	}


	version++;

	message->edit(*clients, getVersion(), name, contents);
}

/*
 * Performs the undo operation.
 */
void spreadsheet::undo() {
	// Check for non-empty stack
	if(!undo_stack_cells->empty()){
		string name     = undo_stack_cells->top();
		string contents = undo_stack_contents->top();

		undo_stack_cells->pop();
		undo_stack_contents->pop();

		version++;

		message->undo(*clients, getVersion(), name, contents);
	}
}

void spreadsheet::sync(int client) {
	message->sync(client, getVersion(), *cells);
}

/*
 * Adds a client to the list of clients.
 */
void spreadsheet::add_client(int client) {
	clients->insert(client);
	sync(client);
}

/*
 * Removes a client from the list of clients.
 */
void spreadsheet::remove_client(int client) {
	clients->erase(client);
}

/*
 * Returns the version as a string.
 */
string spreadsheet::getVersion() {
	stringstream ss;
	ss << version;
	return ss.str();
}

/*
 * Sets the contents of a cell to a formula.  Also performs a check to see if the formula
 * causes a circular dependency.
 */
bool spreadsheet::SetCellContents(string name, string contents) {
	bool flag = true;
	// Remove the trailing \n
	contents = contents.substr(0, contents.size()-1);

	// Store old contents and clear previous dependencies
	string oldContents = (*cells)[name];
	(*cells)[name] = contents;
	if (oldContents != "")
		remove_dependency(name);

	vector<string> cellNames = GetVariables(contents);

	for (vector<string>::iterator it = cellNames.begin(); it != cellNames.end(); ++it)
		dg->AddDependency(name, *it);

	set<string> *temp = new set<string>;

	// Check for circular dependency
	temp->insert(name);
	CircularDependency *c = new CircularDependency(dg);

	try {
		c->GetCellsToRecalculate(temp);
	}
	catch (int e) {
		// Revert to old contents

		// Check if old contents was a formula
		if (oldContents[0] == '=')
			SetCellContents(name, oldContents);
		else
			(*cells)[name] = oldContents;

		flag = false;
	}
	delete c;
	delete temp;
	return flag;
}

/*
 * Removes all dependencies attached to this cell.
 */
void spreadsheet::remove_dependency(string name) {
	if (dg->HasDependents(name)) {
		set<string> dgDependents = dg->GetDependents(name);
		for (set<string>::iterator it = dgDependents.begin(); it != dgDependents.end(); ++it)
			dg->RemoveDependency(name, *it);
	}
}

/*
 * Takes a formula and breaks it into a list of cells.  Each token in the formula must
 * have a colon in between every other token.
 */
vector<string> spreadsheet::GetVariables(string s) {
	vector<string> result;
	
	int i = 0;
	// Loop through entire formula
	while (i < s.size()) {
		// Find where variables start
		if (s[i] >= 'A' && s[i] <= 'z') {
			string temp = "";
			int j = 0;
			// Add each character of the variable to a string and add that to the list
			while ((s[i] >= 'A' && s[i] <= 'z') || (s[i] >= '0' && s[i] <= '9')) {
				stringstream ss;
				string t;
				ss << s[i++];
				ss >> t;
				temp.append(t);
			}
			result.push_back(temp);
		}
		i++;
	}
	
    return result;
}

void spreadsheet::save() {
	ofstream file(filename.c_str());
	if (file.is_open()) {
		file << "<spreadsheet>\n";
		file << "<version>\n";
		file << version << "\n";
		file << "</version>\n";
		for (map<string, string>::iterator it = cells->begin(); it != cells->end(); ++it) {
			file << "<cell>\n";
			file << "<name>\n" << it->first << "\n" << "</name>\n";
			if (it->second.find("\n") != string::npos)
				it->second.erase(it->second.size()-1);
			file << "<contents>\n" << it->second << "\n" << "</contents>\n";
			file << "</cell>\n";
		}
		file << "</spreadsheet>";
		file.close();
	}
	else {
		cout << "Unable to open file." << endl;
	}
}

void spreadsheet::open() {
	string line;
	string name, contents;
	ifstream file(filename.c_str());
	// File already exists
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
}
