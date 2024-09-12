using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// The rest of the namespace and class declaration...

void FixedUpdate()
{
    // The RB3 transform method has been moved to FixedUpdate.
    if(rb3 != null)
    {
        rb3.transform.Translate(0, 0, Time.deltaTime * 2);
    }   
}

void Update()
{
    // The OneShotEvent invocation remains in Update.
    Action ev;
    lock (oneshot_lock)
    {
        ev = oneshot_event;
        if (ev == null)
            return;
        oneshot_event = null;
    }
    ev();
}

// The Play3D method with the Vector3Int parameter should be properly handled instead of always throwing an exception.
// You can either implement the correct logic or remove the Vector3Int overload if it's not needed.
public static void Play3D(string name, Vector3Int position, float spatial_blend = 1f)
{
    // Handling for Vector3Int parameter if necessary or convert to Vector3
    // For this example, conversion to Vector3 is demonstrated below, but you can implement as needed.
    Vector3 positionV3 = new Vector3(position.x, position.y, position.z);
    Play3D(name, positionV3, spatial_blend);
    // throw new Exception("Don't call Play3D with a Vector3Int!");
}

