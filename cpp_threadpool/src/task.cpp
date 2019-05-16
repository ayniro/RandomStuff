#include "task.h"

Task::Task(){
    pthread_mutex_init(&taskmutex, NULL);
    pthread_cond_init(&taskcond, NULL);
    done = false;
}

void Task::run(){
    pthread_mutex_lock(&taskmutex);
    f(arg);
    done = true;
    pthread_cond_signal(&taskcond);
    pthread_mutex_unlock(&taskmutex);
}

void Task::wait(){
    pthread_mutex_lock(&taskmutex);
    while (!done){
        pthread_cond_wait(&taskcond, &taskmutex);
    }
    pthread_mutex_unlock(&taskmutex);
}

Task::~Task(){
    pthread_mutex_destroy(&taskmutex);
    pthread_cond_destroy(&taskcond);
}

