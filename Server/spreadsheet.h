#ifndef SPREADSHEET_H
#define SPREADSHEET_H

/*
 * Represents a working spreadsheet.  It has information on current clients working on it,
 * the cell information, and the version the spreadsheet is on.
 */
class spreadsheet {
	private:
		std::set<int> *clients;
		std::map<std::string, std::string> *cells;
		std::stack<std::string> *undo_stack;
	public:
		spreadsheet();
		~spreadsheet();
		void make_change(std::string);	// Make change to spreadsheet, making all of the necessary checks
		void add_client(int);			// Add a client to the working spreadsheet
		void remove_client(int);		// Remove a client from the working spreadsheet
		void get_version();				// Returns the spreadsheet version
		void undo();					// Performs the undo operation
};

#endif
