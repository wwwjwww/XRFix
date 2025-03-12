

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
        // Here we use a static dictionary to store the pooled objects
        private static readonly Dictionary<Type, List<T>> PoolDict = new Dictionary<Type, List<T>>();

        // Here we use a static List to store all the objects that have been allocated from the pool
        private static readonly List<object> PoolObjects = new List<object>();

        // Here we use a static field to store the number of active objects in the pool
        private static int ActiveCount = 0;

        // Here we use a static method to allocate a new object from the pool
        public static T Allocate()
        {
            T obj;
            if (!PoolDict.TryGetValue(typeof(T), out List<T> list))
            {
                list = new List<T>();
                PoolDict.Add(typeof(T), list);
            }

            if (list.Count == 0)
            {
                ActiveCount++;
                obj = new T();
                PoolObjects.Add(obj);
            }
            else
            {
                obj = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
            }

            return obj;
        }

        // Here we use a static method to release an object back to the pool
        public static void Release(T obj)
        {
            if (PoolDict.ContainsKey(typeof(T)) && PoolDict[typeof(T)].Count > 0)
            {
                PoolDict[typeof(T)].Add(obj);
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
