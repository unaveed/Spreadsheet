#include <sys/socket.h>
#include <sstream>

char * PORT = "12345"; // port numbers 1-1024 are probably reserved by your OS
int MAXLEN = 1024;   // Max lenhgt of a message.
int MAXFD = 7;       // Maximum file descriptors to use. Equals maximum clients.
int BACKLOG = 5;     // Number of connections that can wait in que before they be accepted

volatile fd_set the_state;

pthread_mutex_t mutex_state = PTHREAD_MUTEX_INITIALIZER;

pthread_mutex_t boardmutex = PTHREAD_MUTEX_INITIALIZER; // mutex locker
