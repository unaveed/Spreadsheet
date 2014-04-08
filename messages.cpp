#include "messages.h"

Messages::Messages(std::string incommingLine)
{
    this->input = incommingLine;
    this->delimiter = "[esc]";
}
Messages::Messages(std::string _command, std::string _content){
	this->command = _command;
	this->content = _content;
	this->delimiter = "[esc]";
}
void Messages::send_message(){
	std::string result;
	for(int i = 0; i < command.size(); i++)
		command[i] = toupper(command[i]);
	
	result.append(command);
	result.append(delimiter);
	result.append(content);
	result.append("\n");
	
	// For debugging only	
	std::cout << result << std::endl;

	// TODO: Send the string to the socket
}
void Messages::receive_message(){
    // Hold values of tokens as string as split
    std::string token;

    int escCount = 0;
	size_t index = 0;
	std::string temp = input;
	// Find how many escape characters are in the message
	while((index = temp.find(delimiter)) != std::string::npos){
		temp.erase(0, index + delimiter.size());
		escCount++;
	}

	// For cases where strings contain commands and contents 
	if(escCount > 0){
		index = 0;
		// Keep track of location in the loop
		int count = 1;
		while((index = input.find(delimiter)) != std::string::npos){
			token = input.substr(0, index);
			// Get the command and contents for strings with only one delimiter
			if (escCount == 1){
				command = token;
				input.erase(0, index + delimiter.size());
				content = input;
			}
			// Get the command and contents for strings with severl delimters 
			else {
				if(count == 1)
					command = token;
				else
					content.append(token);
				input.erase(0, index + delimiter.size());
			}
			count++;
		}
		if(content[content.size() - 1] == '\n')
			content.erase(content.size() - 1);
	}
	// In cases where only a command is sent without contents,
	// store the command
	else{
		command = input.substr(0, input.find('\n'));
		content = "Place holder";	
	}

	// TODO: Send information to the server 
}
std::string Messages::get_command(){
    return command;
}
std::string Messages::get_content(){
    return content;
}
int main(){
	Messages *m3 = new Messages("INVALID\n");
	m3->receive_message();
	std::cout << "INVLAID command, no content" << std::endl;
	std::cout << m3->get_command() << std::endl;
	std::cout << m3->get_content() << std::endl;


	Messages *message = new Messages("CREATE[esc]spreadsheet_name\n");
    message->receive_message();
	std::cout << "\nCREATE command" << std::endl;
    std::cout << message->get_command() << std::endl;
    std::cout << message->get_content() << std::endl;

	Messages *m1 = new Messages("ERROR[esc]error_message\n");
	m1->receive_message();
	std::cout << "\nERROR command" << std::endl;
	std::cout << m1->get_command() << std::endl;
	std::cout << m1->get_content() << std::endl;
    
	
	Messages *m2 = new Messages("ENTER[esc]cell_name[esc]cell_content [esc]spreadsheet_name\n");
	m2->receive_message();
	std::cout << "\nENTER command" << std::endl;
	std::cout << m2->get_command() << std::endl;
	std::cout << m2->get_content() << std::endl;
	
	Messages *m4 = new Messages("enter", "cell_name[esc]cell_content");
	m4->send_message();

	delete message;
	delete m1;
	delete m2;
	delete m3;
	delete m4;

	return 0;
}
