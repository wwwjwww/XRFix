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



    // Start is called before the first frame update
    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }

    void Update() {
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         if (ovrHand.IsTracked) {
        //             hand.transform.GetChild(0).gameObject.SetActive(true);
        //             controller.SetActive(false);
        //         } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
        //             controller.SetActive(true);
        //             hand.transform.GetChild(0).gameObject.SetActive(false);
        //         }
        //         rb1.transform.Rotate(30, 0, 0);
        // 
        //         GameObject a2 = Instantiate(gobj2);
        //         var obj2 = a2.AddComponent<Slice>();
        //         obj2.DisposeObj();

        // FIXED VERSION:
