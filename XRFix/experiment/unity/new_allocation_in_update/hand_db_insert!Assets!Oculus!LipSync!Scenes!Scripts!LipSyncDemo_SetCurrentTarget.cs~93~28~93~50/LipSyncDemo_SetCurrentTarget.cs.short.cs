using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

    void Update ()
    {
        // BUG: Using New() allocation in Update() method.
        // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
        //         var inputDevices = new List<InputDevice>();

        // FIXED CODE:
