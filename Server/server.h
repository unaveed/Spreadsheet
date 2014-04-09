#ifndef SERVER_H
#define SERVER_H
class server {
	private:
	public:
		server();
		void run_server();
		void send_message_all(std::string);
		void send_message_client(std::string);
};
#endif
