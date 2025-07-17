using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    void Update()
    {
                // BUG: Using New() allocation in Update() method.
                // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                //                 List<int> triangles = new List<int>();

                // FIXED CODE:
