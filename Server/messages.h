#ifndef MESSAGES_H
#define MESSAGES_H
#include <iostream>
#include <string>
#include <map>
#include "server.h"

class Messages
{
public:
	Messages(server &);
    void receive_message(std::string, std::string &, std::string &);
	void edit(std::set<int> &, std::string, std::string, std::string);
	void sync(std::map<std::string, std::string> &, int);
	void undo(std::string, std::string);
	void save(int);
	void error(std::string, int);
private:
    std::string delimiter;
    std::string command;
    std::string content;
	server main_server;
	bool valid_protocol(std::string);
	int delimiter_count(std::string, std::string);
};

#endif // MESSAGES_H
