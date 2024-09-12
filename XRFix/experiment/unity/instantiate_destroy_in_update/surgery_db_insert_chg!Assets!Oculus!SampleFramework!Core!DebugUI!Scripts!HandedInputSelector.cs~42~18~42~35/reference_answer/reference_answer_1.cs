using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class HandedInputSelector : MonoBehaviour
{
    OVRCameraRig m_CameraRig;
    OVRInputModule m_InputModule;

    protected GameObject gobj1;

    protected GameObject a1;
    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;


    void Start()
    {
        m_CameraRig = FindObjectOfType<OVRCameraRig>();
        m_InputModule = FindObjectOfType<OVRInputModule>();
    }

    private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj1);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a1 = objectPool.Dequeue();
        a1.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a1.SetActive(false);
        objectPool.Enqueue(a1);
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