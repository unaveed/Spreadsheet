#include <set>
#include <map>
#include <stack>
#include <string>

#include "spreadsheet.h"

using namespace std;

spreadsheet::spreadsheet() {
	clients    = new set<int>;
	cells      = new map<string, string>;
	undo_stack = new stack<string>;
	version    = 1;
}

spreadsheet::~spreadsheet() {
	delete clients;
	delete cells;
	delete undo_stack;
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
