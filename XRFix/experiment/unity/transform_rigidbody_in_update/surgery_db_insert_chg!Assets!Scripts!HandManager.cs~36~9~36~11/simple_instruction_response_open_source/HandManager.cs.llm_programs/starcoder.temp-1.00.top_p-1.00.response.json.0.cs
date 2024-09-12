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

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            timer = 0f;
            instantiate_gobj = true;
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            //ResetTimer();
        }
        else if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            //InstantiateNewGameObject();
        }
        timer += Time.deltaTime;
        if (timer > timeLimit && instantiate_gobj)
        {
            InstantiateNewGameObject();
        }
    }

    private void InstantiateNewGameObject()
    {
        Instantiate(gobj2, transform.position, transform.rotation);
        instantiate_gobj = false;
    }

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }
}

    }
}
