using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    // Create a pool of game objects to instantiate.
    private List<GameObject> gameObjectsPool = new List<GameObject>();

    void Start()
    {
        // Add 3 game objects into the pool.
        for (int i = 0; i < 3; i++)
        {
            GameObject obj = new GameObject("Cube");
            gameObjectsPool.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            // Instantiate an object from the pool.
            Instantiate(gameObjectsPool[0]);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            // Destroy the instantiated object.
            Destroy(gameObjectsPool[0]);
            timer = 0;
            instantiate_gobj = false;
        }
    }
}

    }
}
