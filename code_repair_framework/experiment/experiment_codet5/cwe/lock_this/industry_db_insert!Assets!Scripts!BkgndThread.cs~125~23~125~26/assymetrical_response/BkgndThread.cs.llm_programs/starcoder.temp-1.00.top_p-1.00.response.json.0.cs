using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public static class BkgndThread
{
    public class Dispatcher
    {
        /* Create one Dispatcher instance per independent queue of requests.
         * Each Dispatcher holds only a queue of length one, so unprocessed requests
         * will be overwritten with newer requests.  The actual implementation will
         * round-robin between all Dispatchers.
         */

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

        public Compute(RunDelegateWithCompute run_delegate)
        {
            Schedule(() =>
            {
                T result1 = run_delegate(this);
                lock (this)
                {
                    result = result1;
                    got_answer = true;
                }
            });
        }

/*         public bool TryGetAnswer(out T answer)
*         {
*             if (got_answer)
*             {
                * BUG: Locking the 'this' object in a lock statement
                * MESSAGE: It is bad practice to lock the 'this' object because it might be locked elsewhere.
                *                 lock (this)
                *                     answer = result;
                *                 return true;
                *             }
                *             else
                *             {
                *                 answer = default(T);
                *                 return false;
                *             }
                *         }

                * Create a private readonly Object which is used exclusively for locking. This ensures that no other classes can use the same lock.
                * FIXED CODE:
                */

                
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class BkgndThread
{
    public static class Dispatcher
    {
        private static Queue<ThreadStart> _requestQueue;
        private static SemaphoreSlim _semaphore;
        private static AutoResetEvent _event = new AutoResetEvent(false);

        private static bool _bkgnd_thread_dispatcher_working = false;

        static Dispatcher()
        {
            _requestQueue = new Queue<ThreadStart>();
            _semaphore = new SemaphoreSlim(0, int.MaxValue);
            Start(ThreadDispatcher);
        }

        private static void ThreadDispatcher()
        {
            while (true)
            {
                _semaphore.Wait();

                ThreadStart function;
                lock (_requestQueue)
                {
                    function = _requestQueue.Dequeue();
                }
                function();
                _event.Set();
            }
        }

        public static void Schedule(ThreadStart function)
        {
            if (_requestQueue == null)
            {
                _requestQueue = new Queue<ThreadStart>();
                _semaphore = new SemaphoreSlim(0, int.MaxValue);
                Start(ThreadDispatcher);
            }

            lock (_requestQueue)
            {
                if (_bkgnd_thread_dispatcher_working)
                {
                    _requestQueue.Enqueue(function);
                    _semaphore.Release();
                }
                else
                {
                    function();
                    _event.Set();
                }
            }
            _bkgnd_thread_dispatcher_working = true;
        }

        public static void WaitUntilThreadIdle()
        {
            if (_bkgnd_thread_dispatcher_working)
            {
                Schedule(() => { while (_bkgnd_thread_dispatcher_working) { _event.WaitOne(); } });
                _bkgnd_thread_dispatcher_working = false;
            }
        }
    }

    public static class Compute<T>
    {
        public static Compute<U> Create<U>(Func<U> compute_function)
        {
            Compute<U> result = new Compute<U>(() =>
            {
                U result1 = compute_function();
                result.SetResult(result1);
                return result1;
            });
            return result;
        }

        private static object _sync = new object();
        private static Compute<T> _current;
        private static List<Action<T>> _when_completed = new List<Action<T>>();
        private static AutoResetEvent _event = new AutoResetEvent(false);
        private static T _result;
        private static bool _got_answer;
        private static bool _is_interrupted;

        private static void Complete(T result)
        {
            lock (_sync)
            {
                _result = result;
                _got_answer = true;
                if (_is_interrupted)
                {
                    _is_interrupted = false;
                    foreach (var action in _when_completed)
                    {
                        action(default(T));
                    }
                }
                _event.Set();
            }
        }

        public static T GetResult(int timeout_in_ms = -1)
        {
            lock (_sync)
            {
                while (!_got_answer)
                {
                    if (_is_interrupted)
                    {
                        return default(T);
                    }
                    if (timeout_in_ms >= 0)
                    {
                        if (!_event.WaitOne(timeout_in_ms))
                        {
                            return default(T);
                        }
                    }
                    else
                    {
                        _event.WaitOne();
                    }
                }
                return _result;
            }
        }

        public static Compute<T>.Awaiter GetAwaiter()
        {
            return new Awaiter();
        }

        public static void Interrupt()
        {
            lock (_sync)
            {
                if (!_is_interrupted)
                {
                    _is_interrupted = true;
                    _event.Set();
                }
            }
        }

        public static void SetResult(T result)
        {
            Complete(result);
        }

        public class Awaiter : INotifyCompletion
        {
            public bool IsCompleted
            {
                get
                {
                    lock (_sync)
                    {
                        return _got_answer;
                    }
                }
            }

            public T GetResult()
            {
                lock (_sync)
                {
                    while (!_got_answer)
                    {
                        if (_is_interrupted)
                        {
                            return default(T);
                        }
                        _event.WaitOne();
                    }
                    return _result;
                }
            }

            public void OnCompleted(Action continuation)
            {
                Dispatcher.Schedule(() =>
                {
                    T result = default(T);
                    lock (_sync)
                    {
                        if (_got_answer)
                        {
                            result = _result;
                        }
                        else
                        {
                            _when_completed.Add(continuation);
                        }
                    }
                    continuation(result);
                });
            }
        }
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
