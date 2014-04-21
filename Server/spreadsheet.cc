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
		cout << "spreadsheet.cc: make_change: need to sync" << endl;
		cout << "serverVersion:" << sv.str() << " clientVersion:" << vers << endl;
	}

	// Check if contents is a formula
	if (contents[0] == '=') {
		if (!SetCellContents(name, contents)) {
			message->error(client, buildString(contents));
			return;
		}
	}
	else
		(*cells)[name] = contents;

	undo_stack_cells->push(name);
	undo_stack_contents->push(contents);

	// Increment the version (and convert to a string for the message)
	version++;
	cout << "spreadsheet.cc: version=" << getVersion() << endl;

	message->edit(*clients, getVersion(), name, buildString(contents));
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

		message->undo(*clients, getVersion(), name, buildString(contents));
	}
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
			file << "<contents>\n" << it->second  << "</contents>\n";
			file << "</cell>\n";
		}
		file << "</spreadsheet>";
		file.close();
	}
	else
		cout << "Unable to open file." << endl;
}

void spreadsheet::sync(int client) {
	message->sync(client, getVersion(), *cells);
}

/*
 * Takes the contents of a cell and puts it together in regular format.  For example, if
 * the contents are a formula delimited by colons, it will put it back to regular format
 * without the colons.
 */
string spreadsheet::buildString(string contents) {
	if (contents[0] != '=')
		return contents;
	string s;
	vector<string> tokens = GetTokens(contents, ':');
	for (vector<string>::iterator it = tokens.begin(); it != tokens.end(); ++it)
		s.append(*it);
	return s;
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
	// Remove the trailing \n
	contents = contents.substr(0, contents.size()-1);

	// Store old contents and clear previous dependencies
	string oldContents = (*cells)[name];
	if (oldContents != "") {
		remove_dependency(name);
	}

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
		cout << "spreadsheet.cc: Caught exception" << endl;
		// Revert to old contents

		// Check if old contents was a formula
		if (oldContents[0] == '=')
			SetCellContents(name, oldContents);
		else
			(*cells)[name] = oldContents;

		return false;
	}
	delete c;
	delete temp;
	return true;
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
vector<string> spreadsheet::GetVariables(string formula) {
    boost::regex expression("^[_a-zA-Z][a-zA-Z0-9_]*$");
    boost::cmatch what;

    vector<string> v = GetTokens(formula, ':');
    vector<string> vars;

    for (vector<string>::iterator it = v.begin(); it != v.end(); ++it) {
        if(regex_match((*it).c_str(), what, expression))
            vars.push_back(*it);
        else
            continue;
    }   

    return vars;
}



// Cite: http://stackoverflow.com/questions/236129/how-to-split-a-string-in-c
vector<string> spreadsheet::GetTokens(const string &formula, char delim) {
        vector<string> elems;
        split(formula, delim, elems);
        return elems;
}

vector<string> & spreadsheet::split(const string &s, char delim, vector<string> &elems) {
    stringstream ss(s);
    string item;
    while (getline(ss, item, delim))
        elems.push_back(item);
    return elems;
}
// End cite


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
