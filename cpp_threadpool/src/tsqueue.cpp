#include "tsqueue.h"

ThreadsafeQueue::ThreadsafeQueue(){
    pthread_mutex_init(&qmutex, NULL);
    pthread_cond_init(&qcond, NULL);
    stopped = false;
};

void ThreadsafeQueue::push(Task *x){
    pthread_mutex_lock(&qmutex);
    if (!stopped){
        tsqueue.push(x);
        pthread_cond_signal(&qcond);
    }
    pthread_mutex_unlock(&qmutex);
};

Task* ThreadsafeQueue::waitAndPop(){
    Task *task = NULL;
    pthread_mutex_lock(&qmutex);
    
    while (!stopped && tsqueue.empty()) pthread_cond_wait(&qcond, &qmutex);
    
    if (stopped){
        if (tsqueue.empty()){
            pthread_mutex_unlock(&qmutex);
            pthread_exit(NULL);
        }
    }
    task = tsqueue.front();
    tsqueue.pop();
    
    pthread_mutex_unlock(&qmutex);
    return task;
};

void ThreadsafeQueue::stopqueue(){
    pthread_mutex_lock(&qmutex);
    stopped = true;
    pthread_cond_broadcast(&qcond);
    pthread_mutex_unlock(&qmutex);
};

ThreadsafeQueue::~ThreadsafeQueue(){
    pthread_mutex_destroy(&qmutex);
    pthread_cond_destroy(&qcond);
};

