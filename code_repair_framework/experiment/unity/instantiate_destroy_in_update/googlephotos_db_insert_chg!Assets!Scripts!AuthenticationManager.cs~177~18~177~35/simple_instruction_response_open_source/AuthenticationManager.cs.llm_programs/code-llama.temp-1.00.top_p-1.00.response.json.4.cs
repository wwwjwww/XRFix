using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialPoolSize = 10;
    private Stack<GameObject> pool = new Stack<GameObject>();
    private bool instantiated = false;

    void Start()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(false);
            pool.Push(go);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            GameObject go = Instantiate(prefab);
            return go;
        }
        else
        {
            return pool.Pop();
        }
    }

    public void ReleaseObject(GameObject go)
    {
        go.SetActive(false);
        pool.Push(go);
    }
}
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
