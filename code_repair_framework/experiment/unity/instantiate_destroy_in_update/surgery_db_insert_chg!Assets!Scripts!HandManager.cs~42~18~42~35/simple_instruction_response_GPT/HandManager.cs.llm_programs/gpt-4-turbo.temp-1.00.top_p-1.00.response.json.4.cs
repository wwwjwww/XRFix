
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
// Add two new private members for tracking state outside Update()
    private bool handWasTracked;
    private bool controllerWasConnected;

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
        // Initialize these values based on current state at start
        handWasTracked = ovrHand.IsTracked;
        controllerWasConnected = OVRInput.IsControllerConnected(OVRInput.Controller.LTouch);
    }

    void Update() {
        CheckHandStatus();
        RotateRb1();
        TryInstantiateOrUpdateObject();
    }

    private void CheckHandStatus() {
        // Use flags to avoid redundant activation/deactivation
        if (ovrHand.IsTracked && !handWasTracked) {
            hand.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);
            handWasTracked = true;
            controllerWasConnected = false;
        } else if (!ovrHand.IsTracked && OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) && !controllerWasConnected) {
            controller.SetActive(true);
            hand.transform.GetChild(0).gameObject.SetActive(false);
            controllerWasConnected = true;
            handWasTracked = false;
        }
    }

    private void RotateRb1() {
        rb1.transform.Rotate(30 * Time.deltaTime, 0, 0);
    }

    private void TryInstantiateOrUpdateObject() {
        // Timer update mechanism stays the same
        timer += Time.deltaTime;

        // Instantiate only if the flag is false and timer reached the time limit
        if (!instantiate_gobj && timer >= timeLimit) {
            a2 = Instantiate(gobj2);
            timer = 0f;
            instantiate_gobj = true;
        }
        // Dispose of the object correctly without creating a new component
        if (instantiate_gobj && timer >= timeLimit) {
            if (a2 != null) {
                Destroy(a2);
            }
            timer = 0f;
            instantiate_gobj = false;
        }
    }

    // Remove DisposeObj method.

    void OnDestroy() {
        // Use Dispose to ensure gobj2 gets destroyed when this HandManager is destroyed
        if (a2 != null) {
            Destroy(a2);
        }
    }


}
