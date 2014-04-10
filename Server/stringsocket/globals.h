/*
 * Written by Greg Anderson, Umair Naveed, Jesus Zarate, and Celeste Hollenbeck.
 */

#ifndef GLOBALS_H
#define GLOBALS_H

//server constants
extern char * PORT;
extern int MAXLEN;
extern int MAXFD;
extern int BACKLOG;

// This needs to be declared volatile because it can be altered by an other thread. Meaning the compiler cannot
// optimise the code, because it's declared that not only the program can change this variable, but also external
// programs. In this case, a thread.
extern volatile fd_set the_state;

extern pthread_mutex_t mutex_state;

extern pthread_mutex_t boardmutex;



#endif
