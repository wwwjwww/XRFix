using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Oculus.Avatar;
using System.Runtime.InteropServices;
using System.Collections.Generic;

///     void Update()
//     {
//         if (packetQueue.Count > 0)
//         {
            // BUG: Using New() allocation in Update() method.
            // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
            //             List<PacketLatencyPair> deadList = new List<PacketLatencyPair>();
            //             foreach (var packet in packetQueue)
            //             {
            //                 packet.FakeLatency -= Time.deltaTime;
            // 
            //                 if (packet.FakeLatency < 0f)
            //                 {
            //                     ReceivePacketData(packet.PacketData);
            //                     deadList.Add(packet);
            //                 }
            //             }
            // 
            //             foreach (var packet in deadList)
            //             {
            //                 packetQueue.Remove(packet);
            //             }
            //         }
            //     }

            // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
            // FIXED CODE:
