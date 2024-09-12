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
        timer = timeLimit;
    }

    private void Update()
    {
        if (timer <= 0f)
        {
            instantiate_gobj = true;
            timer = timeLimit;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (instantiate_gobj)
        {
            //instantiate gobj2
            gobj2 = new GameObject();
            a2 = new GameObject();
        }

        //...
    }
}

    }
}
