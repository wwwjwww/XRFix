//Here're the fixed code lines from /Assets/Scripts/HandManager.cs:
using System.Collections;
using System.Collections.Generic;
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

    GameObject poolObj;
    Queue<GameObject> objPool = new Queue<GameObject>();
    int poolCapacity = 5;

    private void Awake()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            // create gameobjects and add them to the pool
            var obj = new GameObject("obj" + i);
            obj.SetActive(false);
            objPool.Enqueue(obj);
        }
    }

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }

    void Update()
    {
        if (ovrHand.IsTracked)
        {
            hand.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);
        }
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
        {
            controller.SetActive(true);
            hand.transform.GetChild(0).gameObject.SetActive(false);
        }
        rb1.transform.Rotate(30, 0, 0);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            // get an inactive object from the pool
            if (objPool.Count > 0)
            {
                poolObj = objPool.Dequeue();
                poolObj.SetActive(true);
            }
            // instantiate the new object and add it to the pool
            else
            {
                poolObj = new GameObject("obj");
                poolObj.SetActive(true);
            }
            a2 = poolObj;
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            // destroy the instantiated object
            var obj2 = a2.AddComponent<Slice>();
            obj2.DisposeObj();
            // return the object to the pool
            poolObj.SetActive(false);
            objPool.Enqueue(poolObj);
            timer = 0;
            instantiate_gobj = false;
        }
    }

    //Here's the definition of function call in another component.
    //Related code from /Assets/Scripts/Slice.cs:


        void OnTriggerStay(Collider collider)
        {
            if (collider.name == "Scalpel point")
            {
                Debug.Log("cutting");
                RaycastHit hit;
                var point_transform = collider.transform;
                if (meshCollider.Raycast(new Ray(point_transform.position, point_transform.forward *.02f), out hit, 1))
                {
                    Debug.DrawLine(point_transform.position, hit.point, Color.red, 1);
                    List<int> triangles = new List<int>();
                    triangles.AddRange(mesh.triangles);
                    int startIndex = hit.triangleIndex * 3;
                    triangles.RemoveRange(startIndex, 3)
    }
}
