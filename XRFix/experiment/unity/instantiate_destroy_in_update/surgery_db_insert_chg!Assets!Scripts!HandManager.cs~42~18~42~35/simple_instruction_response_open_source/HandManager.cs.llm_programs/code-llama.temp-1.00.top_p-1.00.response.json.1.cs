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
//*/
public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;
    protected Rigidbody rb1;

    private GameObject a2;

    private float timeLimit = 5f;
    private float timer = 0f;

    private void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
        a2 = new GameObject();
    }

    void Update()
    {
        if (ovrHand.IsTracked)
        {
            // Use the object pool to create a new "dummy" object that can be used for instantiation.
            a2.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);
        }
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
        {
            // Use the object pool to create a new "dummy" object that can be used for instantiation.
            a2.transform.GetChild(0).gameObject.SetActive(false);
            controller.SetActive(true);
        }

        rb1.transform.Rotate(30, 0, 0);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            // Use the object pool to create a new "slice" object that can be used for instantiation.
            a2.AddComponent<Slice>().DisposeObj();
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            // Use the object pool to create a new "slice" object that can be used for instantiation.
            var obj2 = a2.AddComponent<Slice>();
            obj2.DisposeObj();
            timer = 0;
            instantiate_gobj = false;
        }
    }
}

    }
}
