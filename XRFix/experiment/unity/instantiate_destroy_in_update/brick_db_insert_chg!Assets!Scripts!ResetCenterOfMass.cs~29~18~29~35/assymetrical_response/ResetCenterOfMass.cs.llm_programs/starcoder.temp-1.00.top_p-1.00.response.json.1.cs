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

    private Queue<GameObject> objPool;

    void Start()
    {
        objPool = new Queue<GameObject>();
        //add game objects to object pool
        objPool.Enqueue(Instantiate(gobj6));
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a6 = objPool.Dequeue();
            a6.SetActive(true);
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            a6.SetActive(false);
            objPool.Enqueue(a6);
            instantiate_gobj = false;
        }
    }
}

    }
}
