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

    
/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     void Update()
*     {
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit)
*         {
*             a6 = Instantiate(gobj6);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit )
*         {
*             Destroy(a6);
*             timer = 0;
*             instantiate_gobj = false;
*         }
*         

* you can try to build an object pool before Update() method has been called.
* FIXED CODE:
*/

void Update()
{
    // You can try to build an object pool before Update() method has been called
    ObjectPool<GameObject> a6ObjectPool = new ObjectPool<GameObject>(gobj6, 10); // 10 is the initial capacity of the object pool

    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a6 = a6ObjectPool.GetObject(); // Get an object from the pool
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        a6ObjectPool.ReturnObject(a6); // Return the object to the pool
        timer = 0;
        instantiate_gobj = false;
    }
}
    }
}
