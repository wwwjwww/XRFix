

using System;
using System.Collections;
using System.Collections.Generic;




internal static class OVRObjectPool
{
    private static class Storage<T> where T : class, new()
    {
        
        
using System;
using System.Collections;
using System.Collections.Generic;

internal static class OVRObjectPool
{
    private static class Storage<T> where T : class, new()
    {
        private static List<T> _pool = new List<T>();
        private static int _numActive = 0;

        internal static void Add(T obj)
        {
            _pool.Add(obj);
            _numActive++;
        }

        internal static T Get()
        {
            if (_pool.Count == 0)
            {
                return new T();
            }
            else
            {
                T obj = _pool[_pool.Count - 1];
                _pool.RemoveAt(_pool.Count - 1);
                _numActive--;
                return obj;
            }
        }

        internal static void Return(T obj)
        {
            if (_pool.Count < _numActive)
            {
                _pool.Add(obj);
                _numActive++;
            }
        }
    }
}

    }

    /// <summary>
    /// Gets an object of type T from it's respective pool. If none is available a new one is created.
    /// </summary>
    /// <returns>Object of type T</returns>
    public static T Get<T>() where T : class, new()
    {
        using var enumerator = Storage<T>.HashSet.GetEnumerator();
        if (!enumerator.MoveNext()) return new T();
        var item = enumerator.Current;
        Storage<T>.HashSet.Remove(item);

        if (item is IList list) list.Clear();
        else if (item is IDictionary dict) dict.Clear();

        return item;
    }

    public static List<T> List<T>() => Get<List<T>>();

    public static Dictionary<TKey, TValue> Dictionary<TKey, TValue>() => Get<Dictionary<TKey, TValue>>();

    public static HashSet<T> HashSet<T>()
    {
        var item = Get<HashSet<T>>();
        item.Clear();
        return item;
    }

    public static Stack<T> Stack<T>()
    {
        var item = Get<Stack<T>>();
        item.Clear();
        return item;
    }

    public static Queue<T> Queue<T>()
    {
        var item = Get<Queue<T>>();
        item.Clear();
        return item;
    }

    /// <summary>
    /// Returns an object of type T to it's respective pool. If the object is null or already present in the pool no changes are made.
    /// </summary>
    /// <remarks>
    /// After returning an object to the object pool using it is not allowed and leads to undefined behaviour, please <see cref="Get{T}"/> another object from the pool instead.
    /// </remarks>
    public static void Return<T>(T obj) where T : class, new()
    {
        switch (obj)
        {
            case null: return;
            case IList list:
                list.Clear();
                break;
            case IDictionary dict:
                dict.Clear();
                break;
        }

        Storage<T>.HashSet.Add(obj);
    }

    public static void Return<T>(HashSet<T> set)
    {
        set?.Clear();
        Return<HashSet<T>>(set);
    }

    public static void Return<T>(Stack<T> stack)
    {
        stack?.Clear();
        Return<Stack<T>>(stack);
    }

    public static void Return<T>(Queue<T> queue)
    {
        queue?.Clear();
        Return<Queue<T>>(queue);
    }

    public struct ListScope<T> : IDisposable
    {
        List<T> _list;
        public ListScope(out List<T> list) => _list = list = List<T>();
        public void Dispose() => Return(_list);
    }

    public readonly struct DictionaryScope<TKey, TValue> : IDisposable
    {
        readonly Dictionary<TKey, TValue> _dictionary;

        public DictionaryScope(out Dictionary<TKey, TValue> dictionary)
            => _dictionary = dictionary = Dictionary<TKey, TValue>();

        public void Dispose() => Return(_dictionary);
    }

    public readonly struct HashSetScope<T> : IDisposable
    {
        readonly HashSet<T> _set;
        public HashSetScope(out HashSet<T> set) => _set = set = HashSet<T>();
        public void Dispose() => Return(_set);
    }

    public readonly struct StackScope<T> : IDisposable
    {
        readonly Stack<T> _stack;
        public StackScope(out Stack<T> stack) => _stack = stack = Stack<T>();
        public void Dispose() => Return(_stack);
    }

    public readonly struct QueueScope<T> : IDisposable
    {
        readonly Queue<T> _queue;
        public QueueScope(out Queue<T> queue) => _queue = queue = Queue<T>();
        public void Dispose() => Return(_queue);
    }

    public readonly struct ItemScope<T> : IDisposable where T : class, new()
    {
        readonly T _item;
        public ItemScope(out T item) => _item = item = Get<T>();
        public void Dispose() => Return(_item);
    }

}
