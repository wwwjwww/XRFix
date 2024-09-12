using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{

    [Header("Pool Settings")]
    public GameObject prefabToPool;
    public int initialPoolSize = 5;
    public bool growOnDemand = true;

    private Queue<GameObject> pooledObjects = new Queue<GameObject>();
    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    protected void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool);
            pooledObjects.Enqueue(obj);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            GameObject obj = pooledObjects.Dequeue();
            obj.SetActive(true);
            a6 = obj;
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            a6.SetActive(false);
            pooledObjects.Enqueue(a6);
            timer = 0;
            instantiate_gobj = false;
        }
    }
}

Here I used a simple object pool to instantiate and destroy game objects instead of using Instantiate() and Destroy(). The Update() method is still used to control the timeLimit, but now the expensive Instantiate() and Destroy() calls are moved to the object pool
    }
}
