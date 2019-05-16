#include "thread_pool.h"

void* execute(void* arg){
    ThreadPool* tp = (ThreadPool*)(arg);
    while(1){
        Task *t = NULL;
        t = tp->taskqueue.waitAndPop();
        t->run();
    }

    return NULL;
}

ThreadPool::ThreadPool(std::size_t threads_nm){
    finit_called = false;
    pthread_mutex_init(&tpmutex, NULL);
    pthread_cond_init(&tpcond, NULL);
    threads.resize(threads_nm);

    for (std::size_t i = 0; i < threads.size(); i++){
        pthread_create(&threads[i], NULL, execute, (void*)this);
    }
};

void ThreadPool::submit(Task* task){
    taskqueue.push(task);
};

void ThreadPool::finit(){    
    taskqueue.stopqueue();

    for (std::size_t i = 0; i < threads.size(); i++){
        pthread_join(threads[i], NULL);
    }

};

ThreadPool::~ThreadPool(){
    pthread_mutex_destroy(&tpmutex);
    pthread_cond_destroy(&tpcond);
};

