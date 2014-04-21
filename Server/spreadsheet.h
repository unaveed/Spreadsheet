/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#ifndef SPREADSHEET_H
#define SPREADSHEET_H

#include <map>
#include <stack>
#include <vector>
#include <set>

class Messages;
class DependencyGraph;

/*
 * Represents a working spreadsheet.  It has information on current clients working on it,
 * the cell information, and the version the spreadsheet is on.
 */
class spreadsheet {
	private:
		std::set<int> *clients;						// All of the clients currently using this spreadsheet
		std::map<std::string, std::string> *cells;	// All of the cells with their contents
		std::stack<std::string> *undo_stack_contents;		// All changes that have been made (cell name)
		std::stack<std::string> *undo_stack_cells;			// All changes that have been made (cell contents)
		int version;		// Version of the spreadsheet
		//const char * filename;	// Current name of the file
		std::string filename;	// Current name of the file
		Messages * message;	// Object used to communicate with the clients
		DependencyGraph * dg;	// Dependency graph used to detect circular dependencies

		std::vector<std::string> GetTokens(const std::string &formula, char delim);
		std::vector<std::string> &split(const std::string &s, char delim, std::vector<std::string> &elems);
		std::vector<std::string> GetVariables(std::string formula);
		void remove_dependency(std::string name);
		bool SetCellContents(std::string, std::string);	// Sets the contents of the cell to the contents
		std::string getVersion();

	public:
		spreadsheet(std::string filename, Messages *, bool exists);	// Takes the name of the file to open
		~spreadsheet();
		void make_change(int client, std::string name, std::string contents, std::string vers);	// Make change to spreadsheet, making all of the necessary checks
		void add_client(int);			// Add a client to the working spreadsheet
		void remove_client(int);		// Remove a client from the working spreadsheet
		void undo();					// Performs the undo operation
		void save();		// Saves the spreadsheet to the file with the stored filename in xml format
		void open();		// Fills the data structures and variables with the spreadsheet information
		void sync(int client);
		std::string buildString(std::string formula);
};

#endif
