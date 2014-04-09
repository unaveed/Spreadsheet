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
	bool valid_protocol(std::string);
    std::string get_command();
    std::string get_content();
private:
    std::string input;
    std::string delimiter;
    std::string command;
    std::string content;
    std::string cellName;
    std::string cellContent;
};

#endif // MESSAGES_H
