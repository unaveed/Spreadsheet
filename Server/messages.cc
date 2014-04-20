/*
 * Written by Umair Naveed, Celeste Hollenbeck, Gregory Anderson, Jesus Zarate
 */
#include "messages.h"

Messages::Messages(server & svr){
	this->delimiter = "\e";
	this->main_server = svr;
}

 /*
  * Parses the incoming string into a command and 
  * message. If the command is valid, both the
  * message and command are sent to the server
  * for processing. Blank messages are sent to
  * the server for commands without contents.
  */
void Messages::receive_message(std::string input, 
							   std::string &command, 
							   std::string &content){
	std::cout << "\nValid: " << this->valid_protocol(input) << std::endl;

	if(valid_protocol(input)) {
		
		// Hold values of tokens as string as split
		std::string token;

		// Tracks how many times the delimiter appears in a string
		int escCount = delimiter_count(input, delimiter);

		// For cases where strings contain commands and contents 
		if(escCount > 0){
			size_t index = 0;
			// Keep track of location in the loop
			int count = 1;
			while((index = input.find(delimiter)) != std::string::npos){
				token = input.substr(0, index);
				// Get the command and contents for strings with only one delimiter
				if (escCount == 1){
					command = token;
					input.erase(0, index + delimiter.length());
					content = input;
					input.erase(0, input.length());
				}
				// Get the command and contents for strings with severl delimters 
				else {
					// The loop is at the command portion of the string
					if(count == 1) 
						command = token;
					// The loop is at the first part of the message 
					else if(count == 2)
						content = token;
					// The loop is at a portion of the string where
					// the inlcusion of delimiters is required
					else {
						content.append(delimiter);	
						content.append(token);
					}
					input.erase(0, index + delimiter.size());
				}
				count++;
			}
			// Check if the input string still has content
			if(input.length() > 2) {
				// For cases with multiple delimiters, add delmiter
				if(escCount > 1)
					content.append(delimiter);
				content.append(input);
			}

			// Strip new line off of the message
			if(content[content.size() - 1] == '\n')
				content.erase(content.size() - 1);
		}
		// In cases where only a command is sent without contents,
		// store the command
		else {
			command = input.substr(0, input.find('\n'));
			content = ""; // Change to empty string after debugging	
		}
	}
	else {
		command = "ERROR";
		content = "invalid command";
	}
}

 /* 
  * Receives cell name and cell contents made
  * from a successful edit. Modifies the string
  * so that it adheres to the protocol and can
  * be sent to all clients.
  */
void Messages::edit(std::set<int> & clients, std::string version, std::string name, std::string contents) {
	std::string message = "UPDATE";
	for(int i = 0; i < 4; i++) {
		if(i == 1)
			message.append(version);
		if(i == 2)
			message.append(name);
		if(i == 3) {
			message.append(contents);
			message.append("\n");
			break;
		}

		message.append(delimiter);
	}

	main_server.send_message(clients, message);
}

 /* 
  * Builds an UPDATE command with every cell and it's 
  * contents based off the sheet parameter. Sends the
  * contents the the client.
  */
void Messages::sync(int client, std::map<std::string, std::string> &sheet) {
	std::string message = "UPDATE";

	typedef std::map<std::string, std::string>::iterator it_type;
	for(it_type it = sheet.begin(); it != sheet.end(); it++) {
		message.append(delimiter);
		message.append(it->first);
		message.append(delimiter);
		message.append(it->second);
	}
	message.append("\n");
	
	main_server.send_message_client(message, client);
}

 /*
  * Receives version number and the contents of
  * the last change. String is formatted to adhere
  * to the protocol and sent to all clients.
  */
void Messages::undo(std::set<int> & clients, std::string version, std::string name, std::string contents) {
	edit(clients, version, name, contents);
}

 /* 
  * Sends a message to the client comfirming that 
  * save was successful.
  */
void Messages::save(std::set<int> & clients) {
	main_server.send_message(clients, "SAVED\n");
}

/* 
 * Sends error message to the the client
 */
void Messages::error(std::set<int> & clients, std::string content) {
	std::string message = "ERROR";
	message.append(delimiter);
	message.append(content);
	message.append("\n");

	main_server.send_message(clients, message);
}

void Messages::split_edit(std::string message, std::string &version, std::string &name, std::string &contents) {
	size_t index = 0;
	for(int i = 0; i < 3; i++) {
		if(i == 0) {
			index = message.find(delimiter);
			version = message.substr(0, index);
			message.erase(0, index + delimiter.length());
		}
		if(i == 1) {
			index = message.find(delimiter);
			name = message.substr(0, index);
			message.erase(0, index + delimiter.length());
		}
		else 
			contents = message;	
	}

}

 /* 
  * Checks if the string parameter is valid according
  * to the protocol. Returns true/false based on whether
  * it meets the requirement.
  */
bool Messages::valid_protocol(std::string input){
	int escCount = delimiter_count(input, delimiter);
	
	std::string token;
	size_t index;
	
	// Check for valid messages with [esc] delimiters
	if(escCount > 0){
		index = input.find(delimiter);
		token = input.substr(0, index);
		
		if(token == "UPDATE")
			return true;
		else if (token == "ENTER")
			return true;
		else if (token == "SYNC")
			return true;
		else if (token == "UNDO")
			return true;
		else if (token == "SAVE")
			return true;
		else if (token == "FILELIST")
			return true;
		else if (token == "ERROR")
			return true;
		else if (token == "PASSWORD")
			return true;
		else if (token == "OPEN")
			return true;
		else if (token == "CREATE")
			return true;
		else
			return false;
	}
	// Check for valid messages without [esc] delimiters
	else {
		index = input.find("\n");
		token = input.substr(0, index);

		if(token == "RESYNC")
			return true;
		else if (token == "DISCONNECT")
			return true;
		else if (token == "INVALID")
			return true;
		else if (token == "SAVED")
			return true;
		else 
			return false;
	}
}

 /* 
  * Counts the number of times a given delimiter 
  * appears in the parameter string input. Returns
  * the count.
  */
int Messages::delimiter_count(std::string input, std::string delimiter){
	size_t index;
	int result = 0;

	// Find how many escape characters are in the message
	while((index = input.find(delimiter)) != std::string::npos){
		input.erase(0, index + delimiter.size());
		result++;
	}
	return result;
}

/** DELETE AFTER SUCCESSFUL INTEGRETION WITH SEVER 
int main(){
	Messages *m3 = new Messages();
	std::string command, message;
	m3->receive_message("INVALID\n", command, message);
	std::cout << "Command should == INVALID: " << command << std::endl;
	std::cout << "Message should == Place Holder: " << message << std::endl;

	std::string comm = "BOFFIN";
	std::string cont = "[esc]spreadsheet_name\n";
	std::string line = comm.append(cont);
	Messages *msg = new Messages();
	command = "";
	message = "";
    msg->receive_message(line, command, message);
	std::cout << "Command should be error: " << command << std::endl;

	Messages *m1 = new Messages();
	command = "";
	message = "";
	m1->receive_message("ERROR[esc]error_message\n", command, message);
	std::cout << "Command should be ERROR: " << command << std::endl;
	std::cout << "Message should be error_message: " << message << std::endl;
    
	Messages *m2 = new Messages();
	command = "";
	message = "";
	m2->receive_message("ENTER[esc]cell_name[esc]cell_content[esc]spreadsheet_name\n", command, message);
	std::cout << "ENTER command: " << command << std::endl;
	std::cout << "Message should have cell names and contents: " << message << std::endl;

	return 0;
}
**/
