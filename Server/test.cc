/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#include <iostream>
#include <set>
#include <dirent.h>
#include <string>
#include <cstring>

using namespace std;

string get_files(bool clean) {
	string folder = "files/";
	string files = "";
	DIR *d;
	struct dirent *dir;
	// Flag that allows escape characters can be added after first spreadsheet file 
	int addDelimiter = 0;

	string delimiter = "[esc]";

	// Look in the directory names 'files' for files
	d = opendir("files/");

	if(d) {
		// Holds the name of the file
		string fileName;
		
		while ((dir = readdir(d)) != NULL) {
			if(!clean) {
				fileName = folder;
				fileName.append(string(dir->d_name));
			}
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

int main() {
	string names = get_files(true);
	string james = get_files(false);
}

