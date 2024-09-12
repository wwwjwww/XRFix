using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // The object to instantiate and destroy
    public GameObject gameObject;

    // The maximum size of the object pool
    public int poolSize;

    // The current capacity of the object pool
    public int capacity;

    // The current size of the object pool
    public int size;

    // The objects in the pool
    private List<GameObject> poolObjects = new List<GameObject>();

    // The objects in use
    private List<GameObject> inUseObjects = new List<GameObject>();

    // The next available object in the pool
    private int nextObjectIndex = 0;

    // The update method
    void Update()
    {
        if (poolSize == 0)
        {
            return;
        }

        if (inUseObjects.Count < poolSize && poolObjects.Count < capacity)
        {
            poolObjects.Add(Instantiate(gameObject));
            ++size;
        }

        if (inUseObjects.Count < capacity && poolObjects.Count > 0)
        {
            int index = nextObjectIndex % poolObjects.Count;
            inUseObjects.Add(poolObjects[index]);
            poolObjects.RemoveAt(index);
            --size;
            nextObjectIndex = ++index % poolObjects.Count;
        }

        if (inUseObjects.Count == 0)
        {
            inUseObjects = null;
            poolObjects = null;
            capacity = 0;
            size = 0;
            poolSize = 0;
        }
    }

    // The method to return an object to the pool
    public void Release(GameObject obj)
    {
        inUseObjects.Remove(obj);
        poolObjects.Add(obj);
        ++size;
    }
}
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
