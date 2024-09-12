using UnityEngine;
using UnityEngine.UI;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string name;
        public int size;
        public GameObject prefab;
    }

    public Pool[] pools;

    Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        foreach (Pool pool in pools)
        {
            CreatePool(pool);
        }
    }

    void CreatePool(Pool pool)
    {
        Queue<GameObject> poolQueue = new Queue<GameObject>();

        for (int i = 0; i < pool.size; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }

        _pools.Add(pool.name, poolQueue);
    }

    public GameObject GetPooledObject(string poolName)
    {
        if (_pools.ContainsKey(poolName))
        {
            Queue<GameObject> poolQueue = _pools[poolName];
            if (poolQueue.Count > 0)
            {
                GameObject obj = poolQueue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }

    public void ReturnPooledObject(string poolName, GameObject obj)
    {
        if (_pools.ContainsKey(poolName))
        {
            obj.SetActive(false);
            _pools[poolName].Enqueue(obj);
        }
    }


    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
