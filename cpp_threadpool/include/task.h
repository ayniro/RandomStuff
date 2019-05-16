#ifndef TASK_H
#define TASK_H

#include <pthread.h>

class Task{
public:
    pthread_cond_t taskcond;
    pthread_mutex_t taskmutex;
public:
    void (*f)(void *);
    void* arg;
    bool done;
    
    void run();
    void wait();
    
    Task();
    ~Task();
};
#endif // TASK_H

