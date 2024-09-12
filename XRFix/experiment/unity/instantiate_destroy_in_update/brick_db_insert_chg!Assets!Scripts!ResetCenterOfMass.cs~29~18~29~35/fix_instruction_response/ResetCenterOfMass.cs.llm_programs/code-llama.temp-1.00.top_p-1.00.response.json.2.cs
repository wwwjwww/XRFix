// Here's the fixed code from the /Assets/Scripts/ResetCenterOfMass.cs file:
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

    void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        
        timer += Time.deltaTime;
        
        if (!instantiate_gobj && timer >= timeLimit)
        {
            a6 = Instantiate(gobj6);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit )
        {
            Destroy(a6);
            timer = 0;
            instantiate_gobj = false;
        }
        
    }
}
