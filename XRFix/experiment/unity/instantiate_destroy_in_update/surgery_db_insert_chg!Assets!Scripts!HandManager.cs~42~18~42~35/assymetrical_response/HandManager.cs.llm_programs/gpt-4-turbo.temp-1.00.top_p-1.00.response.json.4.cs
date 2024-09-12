
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
// Fields added for object pooling
private Queue<GameObject> objectPool;
private int poolSize = 10;

void Start()
{
    ovrHand = hand.GetComponent<OVRHand>();

    // Initialize the object pool
    objectPool = new Queue<GameObject>();
    for (int i = 0; i < poolSize; i++)
    {
        GameObject obj = Instantiate(gobj2);
        obj.SetActive(false); // Initially deactivate
        objectPool.Enqueue(obj); // Add to the pool
    }
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
    rb1.transform.Rotate(30 * Time.deltaTime, 0, 0); // Add Time.deltaTime for frame-rate independent rotation

    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit && objectPool.Count > 0)
    {
        a2 = objectPool.Dequeue();
        a2.SetActive(true); // Activate the object when taken from the pool
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit && a2 != null)
    {
        var sliceComponent = a2.GetComponent<Slice>();
        sliceComponent.DisposeObj(); // Dispose should deactivate and return to pool instead of destroy
        timer = 0;
        instantiate_gobj = false;
    }
}

// Modified DisposeObj function in the Slice class
public void DisposeObj()
{
    gameObject.SetActive(false); // Deactivate instead of destroying
    objectPool.Enqueue(gameObject); // Return to pool
}

}
