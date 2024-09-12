
//Here're the buggy code lines from /Assets/Scripts/HandManager.cs:
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

// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update() {
//        if (ovrHand.IsTracked) {
//            hand.transform.GetChild(0).gameObject.SetActive(true);
//            controller.SetActive(false);
//        } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
//            controller.SetActive(true);
//            hand.transform.GetChild(0).gameObject.SetActive(false);
//        }
//        rb1.transform.Rotate(30, 0, 0);
//
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a2 = Instantiate(gobj2);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            var obj2 = a2.AddComponent<Slice>();
//            obj2.DisposeObj();
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//
void Update() {
    bool isHandTracked = ovrHand.IsTracked;
    bool isControllerConnected = OVRInput.IsControllerConnected(OVRInput.Controller.LTouch);
    hand.transform.GetChild(0).gameObject.SetActive(isHandTracked);
    controller.SetActive(!isHandTracked && isControllerConnected);

    rb1.transform.Rotate(30 * Time.deltaTime, 0, 0); // Rotate at 30 degrees/second

    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a2 = Instantiate(gobj2);
        timer = 0;
        instantiate_gobj = true;
    }
    else if (instantiate_gobj && timer >= timeLimit)
    {
        instantiate_gobj = false;
        timer = 0;
        var obj2 = a2.AddComponent<Slice>();
        obj2.Dispose(a2); // Assuming Dispose is changed to public and takes GameObject parameter
    }
}

public void DisposeObj()
{
    StartCoroutine(DisposeAfterDelay(gameObject));
}

private IEnumerator DisposeAfterDelay(GameObject objectToDispose)
{
    yield return new WaitForSeconds(timeLimit); // Dispose of the object after timeLimit seconds
    Destroy(objectToDispose);
}


}
