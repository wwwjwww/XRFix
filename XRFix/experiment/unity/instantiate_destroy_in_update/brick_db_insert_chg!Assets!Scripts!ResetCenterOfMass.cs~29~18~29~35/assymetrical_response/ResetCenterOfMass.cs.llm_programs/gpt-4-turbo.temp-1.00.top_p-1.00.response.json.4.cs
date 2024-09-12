/* Here're the buggy code lines from /Assets/Scripts/ResetCenterOfMass.cs:*/
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

[SerializeField]
private GameObject pooledGobj;
private Queue<GameObject> gobjPool;
private int poolSize = 10;

void Start()
{
    PreparePool();
}

void PreparePool()
{
    gobjPool = new Queue<GameObject>();

    for (int i = 0; i < poolSize; i++)
    {
        GameObject gobj = Instantiate(pooledGobj);
        gobj.SetActive(false);
        gobjPool.Enqueue(gobj);
    }
}

void Update()
{
    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        if (gobjPool.Count > 0)
        {
            a6 = gobjPool.Dequeue();
            a6.SetActive(true);
        }
        else
        {
            a6 = Instantiate(pooledGobj);
        }
        
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        a6.SetActive(false);
        gobjPool.Enqueue(a6);
        timer = 0;
        instantiate_gobj = false;
    }
}

    }
}
