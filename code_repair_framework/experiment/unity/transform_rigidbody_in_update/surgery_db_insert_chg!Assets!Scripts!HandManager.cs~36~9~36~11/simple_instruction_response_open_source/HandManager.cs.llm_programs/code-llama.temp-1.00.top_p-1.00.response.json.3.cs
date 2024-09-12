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
    protected OVRHand ovrHand;
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

    void Update()
    {
        if(instantiate_gobj)
        {
            if(timer >= timeLimit)
            {
                Instantiate(gobj2, a2.transform.position, a2.transform.rotation);
                instantiate_gobj = false;
            }

            timer += Time.deltaTime;
        }
    }
}

    }
}
