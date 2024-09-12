using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;
    protected Rigidbody rb1;

    private List<GameObject> gobjPool;
    public GameObject gobjPrefab;
    protected GameObject a2;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
        InitializePool();
    }

    void InitializePool()
    {
        gobjPool = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(gobjPrefab);
            obj.SetActive(false);
            gobjPool.Add(obj);
        }
    }

    GameObject GetPooledObject()
    {
        foreach (GameObject obj in gobjPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Optionally expand the pool if all objects are in use:
        GameObject newObj = Instantiate(gobjPrefab);
        newObj.SetActive(false);
        gobjPool.Add(newObj);
        return newObj;
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
            hand.transform.GetChild(0).gameObject.SetActive(false);
            controller.SetActive(true);
        }
        rb1.transform.Rotate(30, 0, 0);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = GetPooledObject();
            a2.SetActive(true);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            var sliceComponent = a2.GetComponent<Slice>();
            if (sliceComponent != null)
            {
                sliceComponent.DisposeObj();
            }
            else
            {
                // Optionally handle the case when the Slice component is not found
            }
            a2.SetActive(false);
            timer = 0;
            instantiate_gobj = false;
        }
    }

    // Other methods if any...
}
