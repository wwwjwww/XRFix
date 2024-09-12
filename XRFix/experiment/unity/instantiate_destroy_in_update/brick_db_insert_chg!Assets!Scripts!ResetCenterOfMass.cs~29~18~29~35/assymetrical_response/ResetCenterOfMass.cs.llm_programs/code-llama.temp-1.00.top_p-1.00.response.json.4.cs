using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    

    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        // Move the object pool creation to Start() method to avoid instantiating/destroying objects in Update() method.
        CreateObjectPool();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a6 = InstantiateObjectFromPool();
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            DestroyObjectFromPool(a6);
            instantiate_gobj = false;
        }
    }

    // Create an object pool to avoid instantiating/destroying objects in Update() method.
    void CreateObjectPool()
    {
        // Use a list to store the instantiated objects.
        List<GameObject> objectPool = new List<GameObject>();

        // Instantiate objects and store them in the object pool.
        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(gobj6);
            objectPool.Add(go);
        }
    }

    // Instantiate an object from the object pool.
    GameObject InstantiateObjectFromPool()
    {
        // Get an available object from the object pool.
        GameObject go = objectPool[objectPool.Count - 1];

        // Remove the object from the object pool.
        objectPool.RemoveAt(objectPool.Count - 1);

        return go;
    }

    // Destroy an object and add it back to the object pool.
    void DestroyObjectFromPool(GameObject go)
    {
        // Add the object back to the object pool.
        objectPool.Add(go);

        // Destroy the object.
        Destroy(go);
    }
}
    }
}
