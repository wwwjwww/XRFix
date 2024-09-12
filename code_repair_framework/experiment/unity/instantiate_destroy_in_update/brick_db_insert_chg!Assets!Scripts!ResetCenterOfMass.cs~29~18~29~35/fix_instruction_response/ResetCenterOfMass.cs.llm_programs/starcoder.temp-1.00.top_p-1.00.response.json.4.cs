//Here's the fixed code:

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

    private ObjectPool objPool;

    void Start()
    {
        objPool = new ObjectPool();
    }

    void Update()
    {
        if (!instantiate_gobj && timer >= timeLimit)
        {
            a6 = objPool.GetObject();
            Instantiate(a6);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            objPool.ReturnObject(a6);
            a6 = null;
            timer = 0;
            instantiate_gobj = false;
        }
    }

    public class ObjectPool
    {
        private Queue<GameObject> objects = new Queue<GameObject>();

        public void Init(GameObject prefab, int initSize)
        {
            for (int i = 0; i < initSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                objects.Enqueue(obj);
            }
        }

        public GameObject GetObject()
        {
            if (objects.Count > 0)
            {
                return objects.Dequeue();
            }
            else
            {
                return Instantiate(gobj6);
            }
        }

        public void ReturnObject(GameObject obj)
        {
            objects.Enqueue(ob
    }
}
