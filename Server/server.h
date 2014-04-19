/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#include <set>
#include <map>
#include "spreadsheet.h"
#include "messages.h"

#ifndef SERVER_H
#define SERVER_H

class server {
	private:
		std::set<int> *clients;
		std::string delimiter;
		std::string password;
		std::map< std::string, spreadsheet* > *spreadsheets;
		std::map<int, std::string> *clientSpreadsheets;
		std::string get_spreadsheet(int);
		void execute_command(int, std::string, std::string);
		void save_spreadsheets();
	public:
		server();
		~server();
		void run_server();
		void send_message_all(std::string);
		void send_message_client(std::string, int);
		void send_message(std::set<int> &, std::string);
		void message_received(int, std::string, std::string);
		void message_received(int, std::string);
		std::string get_files(bool);	
		void add_client(int);
		void remove_client(int);
};

extern int start(server &);
extern int server_start_listen();
extern int server_establish_connection(int server_fd);
extern int server_send(int fd, std::string data);
extern void *tcp_server_read(void *arg);
extern void mainloop(int server_fd);

#endif
