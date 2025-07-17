using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public static class BkgndThread
{
    public class Dispatcher
    {
        

        ThreadStart pending;

        public void Schedule(ThreadStart function)
        {
            if (requestQueue == null)
                MakeRequestQueue();

            bool must_release = false;
            lock (requestQueue)
            {
                if (pending == null)
                {
                    requestQueue.Enqueue(this);
                    must_release = true;
                }
                pending = function;
            }
            if (must_release)
                semaphore.Release();
            bkgnd_thread_dispatcher_working = true;
        }

        ThreadStart Pop()
        {
            ThreadStart result = pending;
            pending = null;
            return result;
        }

        static Queue<Dispatcher> requestQueue;
        static Semaphore semaphore;
        static bool bkgnd_thread_dispatcher_working = false;

        static void MakeRequestQueue()
        {
            requestQueue = new Queue<Dispatcher>();
            semaphore = new Semaphore(0, int.MaxValue);
            Start(() =>
            {
                while (true)
                {
                    semaphore.WaitOne();

                    Dispatcher dispatcher;
                    ThreadStart function;
                    lock (requestQueue)
                    {
                        dispatcher = requestQueue.Dequeue();
                        function = dispatcher.Pop();
                    }
                    function();
                }
            });
        }

        public static void WaitUntilThreadIdle()
        {
            if (bkgnd_thread_dispatcher_working)
            {
                var wait_until_done = new Dispatcher();
                var ev = new AutoResetEvent(false);
                wait_until_done.Schedule(() => ev.Set());
                ev.WaitOne();
                bkgnd_thread_dispatcher_working = false;
            }
        }
    }


    public class Compute<T> : Dispatcher
    {
        volatile bool got_answer;
        public volatile bool interrupt;
        T result;

        public delegate T RunDelegate();
        public delegate T RunDelegateWithCompute(Compute<T> compute);

        public Compute(RunDelegate run_delegate)
        {
            Schedule(() =>
            {
                T result1 = run_delegate();
                lock (this)
                {
                    result = result1;
                    got_answer = true;
                }
            });
        }

//         public Compute(RunDelegateWithCompute run_delegate)
//         {
//             Schedule(() =>
//             {
//                 T result1 = run_delegate(this);
//                // BUG: Locking the 'this' object in a lock statement
//                // MESSAGE: It is bad practice to lock the 'this' object because it might be locked elsewhere.
//                                 lock (this)
//                                 {
//                                     result = result1;
//                                     got_answer = true;
//                                 }
//                             });
//                         }

//                // Create a private readonly Object which is used exclusively for locking. This ensures that no other classes can use the same lock.
//                // FIXED CODE:

    private readonly object syncRoot = new object(); // Private object to use for locking to avoid external lock conflicts.


    // Existing constructor remains unchanged.
    public Compute(RunDelegate run_delegate)
    {
        Schedule(() =>
        {
            T result1 = run_delegate();
            lock (this)
            {
                result = result1;
                got_answer = true;
            }
        });
    }

    public Compute(RunDelegateWithCompute run_delegate)
    {
        Schedule(() =>
        {
            T result1 = run_delegate(this);
            lock (syncRoot)  // Use the private syncRoot object for locking.
            {
                result = result1;
                got_answer = true;
            }
        });
    }

    // (Rest of the implementation would be here.)




        public bool TryGetAnswer(out T answer)
        {
            if (got_answer)
            {
                lock (this)
                    answer = result;
                return true;
            }
            else
            {
                answer = default(T);
                return false;
            }
        }
    }


    /* Start a new background thread. */
    public static Thread Start(ThreadStart function)
    {
        var th = new Thread(function);
        th.IsBackground = true;
        th.Start();
        return th;
    }
}


public class TwoEndedQueue<T>
{
    /* similar to a thread-safe Queue or the non-available ConcurrentQueue, but simplier:
     * multiple threads can call Enqueue(), but we assume that only one thread calls
     * BlockDequeue().
     */
    List<T> in_list = new List<T>();
    List<T> out_list = new List<T>();
    int out_list_index;
    AutoResetEvent nonempty_event = new AutoResetEvent(false);

    public void Enqueue(T item)
    {
        lock (nonempty_event)
            in_list.Add(item);
        nonempty_event.Set();
    }

    public T BlockDequeue()
    {
        if (out_list_index >= out_list.Count)
        {
            out_list.Clear();
            out_list_index = 0;

            retry:
            nonempty_event.WaitOne();    /* wait for in_list to be non-empty, and then reset */

            List<T> tmp;
            lock (nonempty_event)
            {
                tmp = in_list;
                in_list = out_list;
            }
            out_list = tmp;
            if (out_list.Count == 0)
                goto retry;              /* rare case */
        }

        T result = out_list[out_list_index];
        out_list[out_list_index++] = default(T);
        return result;
    }

    bool Flush()
    {
        if (out_list_index >= out_list.Count)
        {
            out_list.Clear();
            out_list_index = 0;

            List<T> tmp;
            lock (nonempty_event)
            {
                tmp = in_list;
                in_list = out_list;
            }
            out_list = tmp;
            if (out_list.Count == 0)
                return false;
        }
        return true;
    }

    public bool TryDequeue(out T result)
    {
        if (!Flush())
        {
            result = default(T);
            return false;
        }
        result = out_list[out_list_index];
        out_list[out_list_index++] = default(T);
        return true;
    }

    public bool TryPeek(out T result)
    {
        if (!Flush())
        {
            result = default(T);
            return false;
        }
        result = out_list[out_list_index];
        return true;
    }
}
