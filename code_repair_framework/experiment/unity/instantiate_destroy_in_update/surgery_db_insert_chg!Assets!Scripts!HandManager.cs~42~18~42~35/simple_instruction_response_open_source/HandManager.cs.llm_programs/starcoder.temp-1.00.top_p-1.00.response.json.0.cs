using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;
    protected Rigidbody rb1;

    protected GameObject gobj2;
    protected ObjectPool objectPool;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
        objectPool = new ObjectPool(20, gobj2);
    }

    void Update()
    {
        if (ovrHand.IsTracked)
        {
            hand.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);
        }
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
        {
            controller.SetActive(true);
            hand.transform.GetChild(0).gameObject.SetActive(false);
        }
        rb1.transform.Rotate(30, 0, 0);

        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            objectPool.GetObject().SetActive(true);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            objectPool.GetObject().SetActive(false);
            var obj2 = objectPool.GetObject().AddComponent<Slice>();
            obj2.DisposeObj();
            timer = 0;
            instantiate_gobj = false;
        }
    }

    public class ObjectPool
    {
        private List<GameObject> objects = new List<GameObject>();
        private GameObject prefab;
        private int initialPoolSize;

        public ObjectPool(int initialPoolSize, GameObject prefab)
        {
            this.initialPoolSize = initialPoolSize;
            this.prefab = prefab;
            for (int i = 0; i < initialPoolSize; i++)
            {
                objects.Add(Instantiate(prefab));
                objects[i].SetActive(false);
            }
        }

        public GameObject GetObject()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (!objects[i].activeInHierarchy)
                {
                    objects[i].SetActive(true);
                    return objects[i];
                }
            }
            if (objects.Count < initialPoolSize)
            {
                objects.Add(Instantiate(prefab));
            }
            return objects[objects.Count - 1];
        }
    }

    public void Dispose(GameObject obj7)
    {
        objectPool.ReturnObject(obj7);
    }

    public void DisposeObj()
    {
        objectPool.ReturnObject(gameObject);
    }

}
