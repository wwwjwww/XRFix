
/* Here're the buggy code lines from /Assets/Scripts/HandManager.cs:*/
﻿using System.Collections;
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
    protected GameObject a2;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;



    
    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }

/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     void Update() {
*         if (ovrHand.IsTracked) {
*             hand.transform.GetChild(0).gameObject.SetActive(true);
*             controller.SetActive(false);
*         } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
*             controller.SetActive(true);
*             hand.transform.GetChild(0).gameObject.SetActive(false);
*         }
*         rb1.transform.Rotate(30, 0, 0);
* 
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit)
*         {
*             a2 = Instantiate(gobj2);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit )
*         {
*             var obj2 = a2.AddComponent<Slice>();
*             obj2.DisposeObj();
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
/* */
// FIXED CODE for /Assets/Scripts/HandManager.cs

// Add a private field for object pooling
private Queue<GameObject> gobjPool = new Queue<GameObject>();

void Start()
{
    ovrHand = hand.GetComponent<OVRHand>();
    PrepareObjectPool();
}

void PrepareObjectPool()
{
    int poolSize = 10; // or whatever number suits your game
    for (int i = 0; i < poolSize; i++)
    {
        GameObject obj = Instantiate(gobj2); // Use your original prefab
        obj.SetActive(false); // Start with the object deactivated
        gobjPool.Enqueue(obj);
    }
}

GameObject GetPooledObject()
{
    if (gobjPool.Count > 0)
    {
        GameObject obj = gobjPool.Dequeue();
        obj.SetActive(true);
        return obj;
    }
    return null; // In case there are no more objects in the pool
}

void ReleaseObjectToPool(GameObject obj)
{
    obj.SetActive(false);
    gobjPool.Enqueue(obj);
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

    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a2 = GetPooledObject();
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        var obj2 = a2.GetComponent<Slice>();
        obj2.DisposeObj(ReleaseObjectToPool); // Modified to take a callback
        timer = 0;
        instantiate_gobj = false;
    }
}

// FIXED METHOD SIGNATURE inside Slice.cs

public void DisposeObj(Action<GameObject> releaseMethod)
{
    releaseMethod(gameObject);
}


}
