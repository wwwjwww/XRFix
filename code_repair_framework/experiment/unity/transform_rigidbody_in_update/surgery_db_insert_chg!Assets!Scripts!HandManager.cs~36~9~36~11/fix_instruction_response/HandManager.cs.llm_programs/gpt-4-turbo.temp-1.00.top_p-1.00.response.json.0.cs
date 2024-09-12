using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    // ... (Other variables and methods remain unchanged) ...

    // Remove this block of code from Update()
    /*
    rb1.transform.Rotate(30, 0, 0);
    timer+=Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a2 = Instantiate(gobj2);
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        var obj2 = a2.AddComponent<Slice>();
        obj2.DisposeObj();
        timer = 0;
        instantiate_gobj = false;
    }
    */

    // ... (Other methods remain unchanged) ...

    void FixedUpdate()
    {
        // Add this block of code inside FixedUpdate()
        rb1.transform.Rotate(30 * Time.fixedDeltaTime, 0, 0); // Note: modified to account for fixed delta time

        timer += Time.fixedDeltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            var obj2 = a2.AddComponent<Slice>();
            obj2.DisposeObj();
            timer = 0;
            instantiate_gobj = false;
        }
    }
    
    // ... (The rest of the code remains as it was) ...
}

