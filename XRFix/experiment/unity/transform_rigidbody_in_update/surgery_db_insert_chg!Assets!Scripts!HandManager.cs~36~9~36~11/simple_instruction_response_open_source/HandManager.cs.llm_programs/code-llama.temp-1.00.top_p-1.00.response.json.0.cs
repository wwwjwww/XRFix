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



    private void Update()
    {
        if (timer<timeLimit)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (instantiate_gobj)
            {
                Instantiate(gobj2, Vector3.zero, Quaternion.identity);
                instantiate_gobj = false;
            }
            timer = 0f;
        }
        if (ovrHand.GetPressDown(OVRInput.Button.One))
        {
            Debug.Log("Button pressed!");
        }
    }
}

    }
}
