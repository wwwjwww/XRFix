using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    public GameObject prefab;
    private Queue<GameObject> pool = new Queue<GameObject>();
    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            if (pool.Count > 0)
            {
                GameObject gobj = pool.Dequeue();
                gobj.SetActive(true);
                instantiate_gobj = true;
            }
            timer = 0;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            GameObject gobj = pool.Peek();
            if (gobj.activeSelf)
            {
                gobj.SetActive(false);
                pool.Enqueue(gobj);
                instantiate_gobj = false;
            }
            timer = 0;
        }
    }
}

