Here's the fixed code for the 'ResetCenterOfMass.cs' script:

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

/* FIXED CODE:
 *  void Update()
 *  {
 *      timer += Time.deltaTime;
 * 
 *      if (!instantiate_gobj && timer >= timeLimit)
 *      {
 *          a6 = ObjectPool.Instantiate(gobj6);
 *          timer = 0;
 *          instantiate_gobj = true;
 *      }
 *      if (instantiate_gobj && timer >= timeLimit )
 *      {
 *          ObjectPool.Destroy(a6);
 *          timer = 0;
 *          instantiate_gobj = false;
 *      }
 *  }
 *
    }
}
