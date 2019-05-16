#ifndef TSQUEUE_H
#define TSQUEUE_H

#include "task.h"
#include <queue>

class ThreadsafeQueue {
private:
    pthread_mutex_t qmutex;
    pthread_cond_t qcond;
    std::queue<Task*> tsqueue;
public:
    bool stopped;

    void push(Task *x);
    Task* waitAndPop();

    void stopqueue();

    ThreadsafeQueue();
    ~ThreadsafeQueue();
};
#endif // TSQUEUE_H

