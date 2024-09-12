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

    private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj2);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   if (ovrHand.IsTracked) {
        hand.transform.GetChild(0).gameObject.SetActive(true);
        controller.SetActive(false);
   } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
        controller.SetActive(true);
        hand.transform.GetChild(0).gameObject.SetActive(false);
   }
   rb1.transform.Rotate(30, 0, 0);

   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a2 = objectPool.Dequeue();
        a2.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a2.SetActive(false);
        objectPool.Enqueue(a2);
        timer = 0;
        instantiate_gobj = false;
   }
   if(OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
   {
        SetActiveController(OVRInput.Controller.LTouch);
   }
   else
   {
        SetActiveController(OVRInput.Controller.RTouch);
   }
}
}