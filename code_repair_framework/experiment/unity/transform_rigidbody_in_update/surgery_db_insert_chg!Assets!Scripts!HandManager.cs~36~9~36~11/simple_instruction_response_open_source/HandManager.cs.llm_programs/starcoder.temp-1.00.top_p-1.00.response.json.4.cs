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
    protected GameObject a2;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;



    
    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }





// FIXED CODE
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
    protected GameObject a2;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
        rb1 = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!OVRHand.isUserPresent)
        {
            return;
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(hit.transform.gameObject);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            instantiate_gobj = true;
        }
    }

    void FixedUpdate()
    {
        if (!OVRHand.isUserPresent)
        {
            return;
        }

        // Grab objects
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<Rigidbody>()!= null)
                {
                    rb1.isKinematic = false;
                    rb1.AddForce(hit.transform.forward * 200f, ForceMode.Impulse);
    }
}
