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
	void sync(int, std::map<std::string, std::string> &);
	void undo(std::set<int> &, std::string, std::string, std::string);
	void save(std::set<int> &);
	void error(int, std::string);
	void split_edit(std::string, std::string &, std::string &, std::string &);
private:
    std::string delimiter;
    std::string command;
    std::string content;
	server main_server;
	bool valid_protocol(std::string);
	int delimiter_count(std::string, std::string);
};

#endif // MESSAGES_H
