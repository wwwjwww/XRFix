//Here're the buggy code lines from /Assets/Scripts/ResetCenterOfMass.cs:
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    

    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;


    void Start()
    {
        
    }

    
// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a6 = Instantiate(gobj6);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            Destroy(a6);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//        

//you can try to build an object pool before Update() method has been called.
// FIXED CODE:

private Queue<GameObject> objectPool = new Queue<GameObject>();

void Start()
{
    for (int i = 0; i < 10; i++) // Initialize a pool with 10 objects
    {
        GameObject obj = Instantiate(gobj6);
        obj.SetActive(false); // Initially deactivate them
        objectPool.Enqueue(obj);
    }
}

void Update()
{
    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        if (objectPool.Count > 0)
        {
            a6 = objectPool.Dequeue();
            a6.SetActive(true);
            timer = 0;
            instantiate_gobj = true;
        }
    }
    else if (instantiate_gobj && timer >= timeLimit)
    {
        if (a6 != null)
        {
            a6.SetActive(false);
            objectPool.Enqueue(a6);
            timer = 0;
            instantiate_gobj = false;
        }
    }
}

}
