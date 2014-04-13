#ifndef MESSAGES_H
#define MESSAGES_H
#include <iostream>
#include <string>
#include <map>

class Messages
{
public:
	Messages();
    void receive_message(std::string);
	void edit(std::string);
	void sync(std::map<std::string, std::string> &, int);
	void undo(std::string);
	void save(int);
	void error(std::string, int);
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
