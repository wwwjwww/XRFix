using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Oculus.Avatar;
using System.Runtime.InteropServices;
using System.Collections.Generic;

    void Update()
    {
            // BUG: Using New() allocation in Update() method.
            // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
            //             List<PacketLatencyPair> deadList = new List<PacketLatencyPair>();

            // FIXED CODE:
