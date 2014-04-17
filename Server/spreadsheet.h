/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#ifndef SPREADSHEET_H
#define SPREADSHEET_H
#include <map>
#include <stack>

/*
 * Represents a working spreadsheet.  It has information on current clients working on it,
 * the cell information, and the version the spreadsheet is on.
 */
class spreadsheet {
	private:
		std::set<int> *clients;						// All of the clients currently using this spreadsheet
		std::map<std::string, std::string> *cells;	// All of the cells with their contents
		std::stack<std::string> *undo_stack;		// All changes that have been made
		int version;		// Version of the spreadsheet
		char * filename;	// Current name of the file
	public:
		spreadsheet(char *);	// Takes the name of the file to open
		~spreadsheet();
		void make_change(std::string);	// Make change to spreadsheet, making all of the necessary checks
		void add_client(int);			// Add a client to the working spreadsheet
		void remove_client(int);		// Remove a client from the working spreadsheet
		int get_version();				// Returns the spreadsheet version
		void undo();					// Performs the undo operation
		void save();		// Saves the spreadsheet to the file with the stored filename in xml format
		void open();		// Fills the data structures and variables with the spreadsheet information
};

#endif
