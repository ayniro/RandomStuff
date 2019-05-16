#include <iostream>
#include <cstdio>
#include <unistd.h>
#include "thread_pool.h"

void foo(void* arg_){
    printf("got %d\n", *(int*)arg_);
    delete (int*)arg_;
    usleep(500000);
}

int main()
{
    ThreadPool pool(4);
    Task tasks[50];
    for (int i = 0; i < 50; i++){
        tasks[i].f = foo;
        int* arg = new int;
        *arg = i;
        tasks[i].arg = arg;
        pool.submit(&tasks[i]);
    }
    pool.finit();

    return 0;
}

