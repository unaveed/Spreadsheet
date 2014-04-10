#ifndef MESSAGES_H
#define MESSAGES_H
#include <iostream>
#include <string>

class Messages
{
public:
    Messages(std::string);
	Messages(std::string, std::string);
    void send_message();
    void receive_message();
    
	/** Maybe these should be private? **/	
	std::string get_command();
    std::string get_content();
private:
    std::string input;
    std::string delimiter;
    std::string command;
    std::string content;
    std::string cellName;
    std::string cellContent;
	bool valid_protocol(std::string);
	int delimiter_count(std::string, std::string);
};

#endif // MESSAGES_H
