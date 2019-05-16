#ifndef THREAD_POOL_H
#define THREAD_POOL_H

#include "tsqueue.h"
#include <vector>
#include <cstddef>

class ThreadPool{
private:
    std::vector<pthread_t> threads;
public:
    pthread_mutex_t tpmutex;
    pthread_cond_t tpcond;
    ThreadsafeQueue taskqueue;
    
    bool finit_called;
    void submit(Task* task);
    void finit();

    ThreadPool(std::size_t threads_nm);
    ~ThreadPool();
};
#endif // THREAD_POOL_H

