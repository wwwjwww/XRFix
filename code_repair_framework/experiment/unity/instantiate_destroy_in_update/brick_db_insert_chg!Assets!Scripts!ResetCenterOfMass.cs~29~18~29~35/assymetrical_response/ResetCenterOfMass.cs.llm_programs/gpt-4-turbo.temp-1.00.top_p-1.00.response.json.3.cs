using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    private Queue<GameObject> objectPool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < 1; i++) // Assuming we need just 1 object in pool considering the original code
        {
            GameObject obj = Instantiate(gobj6);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            if (objectPool.Count > 0)
            {
                a6 = objectPool.Dequeue();
                a6.SetActive(true);
                timer = 0;
                instantiate_gobj = true;
            }
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            a6.SetActive(false);
            objectPool.Enqueue(a6);
            timer = 0;
            instantiate_gobj = false;
        }
    }
}
    }
}
