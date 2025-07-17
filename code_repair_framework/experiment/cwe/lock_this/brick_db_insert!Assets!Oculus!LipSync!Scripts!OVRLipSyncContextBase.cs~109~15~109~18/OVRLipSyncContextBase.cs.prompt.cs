/************************************************************************************
Filename    :   OVRLipSyncContext.cs
Content     :   Interface to Oculus Lip-Sync engine
Created     :   August 6th, 2015
Copyright   :   Copyright Facebook Technologies, LLC and its affiliates.
                All rights reserved.

Licensed under the Oculus Audio SDK License Version 3.3 (the "License");
you may not use the Oculus Audio SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/audio-3.3/

Unless required by applicable law or agreed to in writing, the Oculus Audio SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
************************************************************************************/
using UnityEngine;


[RequireComponent(typeof(AudioSource))]









public class OVRLipSyncContextBase : MonoBehaviour
{


    public AudioSource audioSource = null;

    [Tooltip("Which lip sync provider to use for viseme computation.")]
    public OVRLipSync.ContextProviders provider = OVRLipSync.ContextProviders.Enhanced;
    [Tooltip("Enable DSP offload on supported Android devices.")]
    public bool enableAcceleration = true;



    private OVRLipSync.Frame frame = new OVRLipSync.Frame();
    private uint context = 0;    // 0 is no context

    private int _smoothing;
    public int Smoothing
    {
        set
        {
            OVRLipSync.Result result =
                OVRLipSync.SendSignal(context, OVRLipSync.Signals.VisemeSmoothing, value, 0);

            if (result != OVRLipSync.Result.Success)
            {
                if (result == OVRLipSync.Result.InvalidParam)
                {
                    Debug.LogError("OVRLipSyncContextBase.SetSmoothing: A viseme smoothing" +
                        " parameter is invalid, it should be between 1 and 100!");
                }
                else
                {
                    Debug.LogError("OVRLipSyncContextBase.SetSmoothing: An unexpected" +
                        " error occured.");
                }
            }

            _smoothing = value;
        }
        get
        {
            return _smoothing;
        }
    }

    public uint Context
    {
        get
        {
            return context;
        }
    }

    protected OVRLipSync.Frame Frame
    {
        get
        {
            return frame;
        }
    }




///     void Awake()
//     {
// 
//         if (!audioSource)
//         {
//             audioSource = GetComponent<AudioSource>();
//         }
// 
        // BUG: Locking the 'this' object in a lock statement
        // MESSAGE: It is bad practice to lock the 'this' object because it might be locked elsewhere.
        //         lock (this)
        //         {
        //             if (context == 0)
        //             {
        //                 if (OVRLipSync.CreateContext(ref context, provider, 0, enableAcceleration)
        //                     != OVRLipSync.Result.Success)
        //                 {
        //                     Debug.LogError("OVRLipSyncContextBase.Start ERROR: Could not create" +
        //                         " Phoneme context.");
        //                     return;
        //                 }
        //             }
        //         }
        //     }

        // FIXED CODE:
