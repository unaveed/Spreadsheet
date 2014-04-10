#include <set>

#ifndef SERVER_H
#define SERVER_H

class server {
	private:
		std::set<int> *clients;

	public:
		server();
		~server();
		void run_server();
		void send_message_all(std::string);
		void send_message_client(std::string, int);
		void message_received(int, std::string);
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