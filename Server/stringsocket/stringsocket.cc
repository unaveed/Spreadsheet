/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */


#include <iostream>
#include <cstring> 	// used for memset.
#include <arpa/inet.h> 	// for inet_ntop function
#include <netdb.h>
#include <unistd.h>

#include <errno.h>

#include "globals.h"
#include "../server.h"


using namespace std;


//server functions
int start(server &);
int server_start_listen();
int server_establish_connection(int server_fd);
int server_send(int fd, std::string data);
void *tcp_server_read(void *arg);
void mainloop(int server_fd);

server * main_server;




int start(server & svr) {
	main_server = &svr;
    cout << "Server started." << endl; // do not forget endl, or it won't display.

    // start the main and make the server listen on port 12345
    // server_start_listen(12345) will return the server's fd.

    int server_fd = server_start_listen() ;
    if (server_fd == -1) {
        cout << "An error occured. Closing program." ;
        return 1;
    }

    mainloop(server_fd);

    return 0;
}

int server_start_listen() {
	struct addrinfo hostinfo, *res;

	int sock_fd;

	int server_fd; // the fd the server listens on
	int ret;
	int yes = 1;

	// first, load up address structs with getaddrinfo():

	memset(&hostinfo, 0, sizeof(hostinfo));

	hostinfo.ai_family = AF_UNSPEC;  // use IPv4 or IPv6, whichever
	hostinfo.ai_socktype = SOCK_STREAM;
	hostinfo.ai_flags = AI_PASSIVE;     // fill in my IP for me

	getaddrinfo(NULL, PORT, &hostinfo, &res);


	server_fd = socket(res->ai_family, res->ai_socktype, res->ai_protocol);

	//prevent "Error Address already in use"
	ret = setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR, &yes, sizeof(int));

	ret = bind(server_fd, res->ai_addr, res->ai_addrlen);

	if(ret != 0) {
		cout << "error :" << strerror(errno) << endl;
		return -1 ;
	}

	ret = listen(server_fd, BACKLOG);

	return server_fd;
}

// This function will establish a connection between the server and the
// client. It will be executed for every new client that connects to the server.
// This functions returns the socket filedescriptor for reading the clients data
// or an error if it failed.
int server_establish_connection(int server_fd) {
    char ipstr[INET6_ADDRSTRLEN];
    int port;

    int new_sd;
    struct sockaddr_storage remote_info ;
    socklen_t addr_size;

    addr_size = sizeof(addr_size);
    new_sd = accept(server_fd, (struct sockaddr *) &remote_info, &addr_size);

    getpeername(new_sd, (struct sockaddr*)&remote_info, &addr_size);

   // deal with both IPv4 and IPv6:
	if (remote_info.ss_family == AF_INET) {
		struct sockaddr_in *s = (struct sockaddr_in *)&remote_info;
		port = ntohs(s->sin_port);
		inet_ntop(AF_INET, &s->sin_addr, ipstr, sizeof ipstr);
	}
	else { // AF_INET6
		struct sockaddr_in6 *s = (struct sockaddr_in6 *)&remote_info;
		port = ntohs(s->sin6_port);
		inet_ntop(AF_INET6, &s->sin6_addr, ipstr, sizeof ipstr);
	}

	cout << "Connection accepted from " << ipstr << " using port " << port << endl;

    return new_sd;
}

// This function will send data to the clients fd.
// data contains the message to be send
int server_send(int fd, string data) {
    int ret;

    ret = send(fd, data.c_str(), strlen(data.c_str()),0);

    return 0;
}

// This function runs in a thread for every client, and reads incoming data.
// It also writes the incoming data to all other clients.
void *tcp_server_read(void *arg) {
    int rfd;

    char buf[MAXLEN];
    int buflen;
    int wfd;

    rfd = (int)arg;
    for(;;) {
        //read incoming message.
        buflen = read(rfd, buf, sizeof(buf));
        if (buflen <= 0) {
            cout << "client disconnected. Clearing fd. " << rfd << endl;
            pthread_mutex_lock(&mutex_state);

			// Remove client from list of clients
			main_server->remove_client(rfd);

            FD_CLR(rfd, &the_state);      // free fd's from  clients
            pthread_mutex_unlock(&mutex_state);
            close(rfd);
            pthread_exit(NULL);
        }

        // send the data to the other connected clients
        pthread_mutex_lock(&mutex_state);

		/*
		// Send message out to all users
		for (wfd = 3; wfd < MAXFD; ++wfd) {
			if (FD_ISSET(wfd, &the_state) && (rfd != wfd)) {
				// add the users FD to the message to give it an unique ID.
				string userfd;
				stringstream out;
				out << wfd;
				userfd = out.str();
				userfd = userfd + ": ";

                server_send(wfd, userfd);
                server_send(wfd, buf);
            }
        }
		*/

		// Create the message to send
		string message;
		for (int i = 0; i < buflen; i++)
			message += buf[i];

		main_server->message_received(rfd, message);

		// Clear the buffer
		memset(&buf[0], 0, buflen);

        pthread_mutex_unlock(&mutex_state);
    }
    return NULL;
}

// This loop will wait for a client to connect. When the client connects, it creates a
// new thread for the client and starts waiting again for a new client.
void mainloop(int server_fd) {
    pthread_t threads[MAXFD]; //create handles for threads.

    FD_ZERO(&the_state); // FD_ZERO clears all the filedescriptors in the file descriptor set fds.

	// start looping here
    while(1) {
        int rfd;
        void *arg; 

        // if a client is trying to connect, establish the connection and create a fd for the client.
        rfd = server_establish_connection(server_fd);

        if (rfd >= 0) {
            cout << "Client connected. Using file desciptor " << rfd << endl;

			// Add client to the list of clients
			main_server->add_client(rfd);

            if (rfd > MAXFD) {
                cout << "To many clients trying to connect." << endl;
                close(rfd);
                continue;
            }

            pthread_mutex_lock(&mutex_state);  // Make sure no 2 threads can create a fd simultanious.

            FD_SET(rfd, &the_state);  // Add a file descriptor to the FD-set.

            pthread_mutex_unlock(&mutex_state); // End the mutex lock

            arg = (void *) rfd;

            // now create a thread for this client to intercept all incoming data from it.
            pthread_create(&threads[rfd], NULL, tcp_server_read, arg);
        }
    }
}
