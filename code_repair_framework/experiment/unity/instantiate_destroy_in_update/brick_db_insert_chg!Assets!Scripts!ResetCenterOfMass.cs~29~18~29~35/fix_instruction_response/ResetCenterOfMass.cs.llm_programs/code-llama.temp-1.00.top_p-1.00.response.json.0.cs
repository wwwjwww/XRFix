using System.Collections;
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
        // Build an object pool before Update() method has been called.
        a6 = GameObject.Instantiate(gobj6);
    }

    // BUG: Instantiate/Destroy in Update() method
    // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
    // void Update()
    // {
    //     timer+=Time.deltaTime;

    //     if (!instantiate_gobj && timer >= timeLimit)
    //     {
    //         Destroy(a6);
    //         instantiate_gobj = false;
    //     }
    //     if (instantiate_gobj && timer >= timeLimit )
    //     {
    //         a6 = GameObject.Instantiate(gobj6);
    //         instantiate_gobj = true;
    //     }
    // }
//you can try to build an object pool before Update() method has been called.
}

