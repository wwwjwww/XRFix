/************************************************************************************
Filename    :   OculusSpatializerUnity.cs
Content     :   Interface into real-time geometry reflection engine for native Unity
Copyright   :   Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus SDK Version 3.5 (the "License"); 
you may not use the Oculus SDK except in compliance with the License, 
which is provided at the time of installation or download, or which 
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/sdk-3.5/

Unless required by applicable law or agreed to in writing, the Oculus SDK 
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class OculusSpatializerUnity : MonoBehaviour
{
    public LayerMask layerMask = -1;
    public bool visualizeRoom = true;
    bool roomVisualizationInitialized = false;

    public int raysPerSecond = 256;
    public float roomInterpSpeed = 0.9f;
    public float maxWallDistance = 50.0f;
    public int rayCacheSize = 512;

    public bool dynamicReflectionsEnabled = true;
    float particleSize = 0.2f;
    float particleOffset = 0.1f;

    GameObject room;
    Renderer[] wallRenderer = new Renderer[6];

    float[] dims = new float[3] { 1.0f, 1.0f, 1.0f };
    float[] coefs = new float[6];

    const int HIT_COUNT = 2048;

    Vector3[] points = new Vector3[HIT_COUNT];
    Vector3[] normals = new Vector3[HIT_COUNT];

    ParticleSystem sys;
    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[HIT_COUNT];


    static LayerMask gLayerMask = -1;
    static Vector3 swapHandedness(Vector3 vec) { return new Vector3(vec.x, vec.y, -vec.z); }
    [MonoPInvokeCallback(typeof(AudioRaycastCallback))]
    static void AudioRaycast(Vector3 origin, Vector3 direction, out Vector3 point, out Vector3 normal, System.IntPtr data)
    {
        point = Vector3.zero;
        normal = Vector3.zero;

        RaycastHit hitInfo;
        if (Physics.Raycast(swapHandedness(origin), swapHandedness(direction), out hitInfo, 1000.0f, gLayerMask.value))
        {
            point = swapHandedness(hitInfo.point);
            normal = swapHandedness(hitInfo.normal);
        }
    }

    void Start()
    {
        OSP_Unity_AssignRaycastCallback(AudioRaycast, System.IntPtr.Zero);
    }

    void OnDestroy()
    {
        OSP_Unity_AssignRaycastCallback(System.IntPtr.Zero, System.IntPtr.Zero);
    }

    void Update()
    {
            // BUG: Using New() allocation in Update() method.
            // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
            //         if (dynamicReflectionsEnabled)
            //         {
            //             OSP_Unity_AssignRaycastCallback(AudioRaycast, System.IntPtr.Zero);
            //         }
            //         else
            //         {
            //             OSP_Unity_AssignRaycastCallback(System.IntPtr.Zero, System.IntPtr.Zero);
            //         }
            // 
            //         OSP_Unity_SetDynamicRoomRaysPerSecond(raysPerSecond);
            //         OSP_Unity_SetDynamicRoomInterpSpeed(roomInterpSpeed);
            //         OSP_Unity_SetDynamicRoomMaxWallDistance(maxWallDistance);
            //         OSP_Unity_SetDynamicRoomRaysRayCacheSize(rayCacheSize);
            // 
            //         gLayerMask = layerMask;
            //         OSP_Unity_UpdateRoomModel(1.0f);
            // 
            //         if (visualizeRoom)
            //         {
            //             if (!roomVisualizationInitialized)
            //             {
            //                 inititalizeRoomVisualization();
            //                 roomVisualizationInitialized = true;
            //             }
            // 
            //             Vector3 pos;
            //             OSP_Unity_GetRoomDimensions(dims, coefs, out pos);
            // 
            //             pos.z *= -1; // swap to left-handed
            // 
            //             var size = new Vector3(dims[0], dims[1], dims[2]);
            // 
            //             float magSqrd = size.sqrMagnitude;
            // 
            //             if (!float.IsNaN(magSqrd) && 0.0f < magSqrd && magSqrd < 1000000.0f)
            //             {
            //                 transform.localScale = size * 0.999f;
            //             }
            // 
            //             transform.position = pos;
            // 
            //             OSP_Unity_GetRaycastHits(points, normals, HIT_COUNT);
            // 
            //             for (int i = 0; i < HIT_COUNT; ++i)
            //             {
            //                 if (points[i] == Vector3.zero)
            //                     points[i].y = -10000.0f; // hide it
            // 
            //                 // swap to left-handed
            //                 points[i].z *= -1;
            //                 normals[i].z *= -1;
            // 
            //                 particles[i].position = points[i] + normals[i] * particleOffset;
            // 
            //                 if (normals[i] != Vector3.zero)
            //                     particles[i].rotation3D = Quaternion.LookRotation(normals[i]).eulerAngles;
            // 
            //                 particles[i].startSize = particleSize;
            //                 particles[i].startColor = new Color(208 / 255f, 38 / 255f, 174 / 255f, 1.0f);
            //             }
            // 
            //             for (int wall = 0; wall < 6; ++wall)
            //             {
            //                 var color = Color.Lerp(Color.red, Color.green, coefs[wall]);
            //                 wallRenderer[wall].material.SetColor("_TintColor", color);
            //             }
            // 
            //             sys.SetParticles(particles, particles.Length);
            //         }
            //     }

            // FIXED VERSION:
