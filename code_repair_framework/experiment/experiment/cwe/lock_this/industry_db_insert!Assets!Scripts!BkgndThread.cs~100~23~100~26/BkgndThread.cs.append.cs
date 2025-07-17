
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
