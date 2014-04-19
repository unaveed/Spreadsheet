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

using namespace std;

spreadsheet::spreadsheet(const char * _filename, Messages * _message) {
	clients    = new set<int>;
	cells      = new map<string, string>;
	undo_stack = new stack<string>;
	version    = 1;
	filename   = _filename;
	message	   = _message;
	dg         = new DependencyGraph();

	open();
}

spreadsheet::~spreadsheet() {
	delete clients;
	delete cells;
	delete undo_stack;
}



/************************* CONVERTING SPREADSHEET FROM C# **********************************/

string spreadsheet::GetCellContents(string name) {
	return (*cells)[name];
}





void spreadsheet::SetCellContents(string name, string contents) {
	dg->ReplaceDependents(name, new set<string>);
	(*cells)[name] = contents;
}

void spreadsheet::SetCellContentsFormula(string name, string contents) {
	dg->ReplaceDependents(name, new set<string>);
	(*cells)[name] = contents;
}

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


/************************* END CONVERTING **********************************/






void spreadsheet::make_change(std::string) {
	version++;
	stringstream ss;
	ss << version;
	//message->edit(ss.str(), name, contents, &clients);
}

void spreadsheet::undo(){
	// Check for non-empty stack
	if(!undo_stack->empty()){
		std::string edit = undo_stack->top();
		undo_stack->pop();

		stringstream ss;
		ss << version;

		//Messages *mess = new Messages("ENTER", edit);
		//mess->send_message();
	}
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

void spreadsheet::add_client(int client) {
	clients->insert(client);
}

void spreadsheet::remove_client(int client) {
	clients->erase(client);
}

int spreadsheet::get_version() {
	return version;
}
