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

//-------------------------------------------------------------------------------------
// ***** OVRLipSyncContext
//
/// <summary>
/// OVRLipSyncContext interfaces into the Oculus phoneme recognizer.
/// This component should be added into the scene once for each Audio Source.
///
/// </summary>
public class OVRLipSyncContext : OVRLipSyncContextBase
{
    // * * * * * * * * * * * * *
    // Public members


    [Tooltip("Allow capturing of keyboard input to control operation.")]
    public bool enableKeyboardInput = false;
    [Tooltip("Register a mouse/touch callback to control loopback and gain (requires script restart).")]
    public bool enableTouchInput = false;
    [Tooltip("Play input audio back through audio output.")]
    public bool audioLoopback = false;
    [Tooltip("Key to toggle audio loopback.")]
    public KeyCode loopbackKey = KeyCode.L;
    [Tooltip("Show viseme scores in an OVRLipSyncDebugConsole display.")]
    public bool showVisemes = false;
    [Tooltip("Key to toggle viseme score display.")]
    public KeyCode debugVisemesKey = KeyCode.D;
    [Tooltip("Skip data from the Audio Source. Use if you intend to pass audio data in manually.")]
    public bool skipAudioSource = false;
    [Tooltip("Adjust the linear audio gain multiplier before processing lipsync")]
    public float gain = 1.0f;

    private bool hasDebugConsole = false;

    public KeyCode debugLaughterKey = KeyCode.H;
    public bool showLaughter = false;
    public float laughterScore = 0.0f;

    // * * * * * * * * * * * * *
    // Private members

    /// <summary>
    /// Start this instance.
    /// Note: make sure to always have a Start function for classes that have editor scripts.
    /// </summary>
    void Start()
    {
        // Add a listener to the OVRTouchpad for touch events
        if (enableTouchInput)
        {
            OVRTouchpad.AddListener(LocalTouchEventCallback);
        }

        // Find console
        OVRLipSyncDebugConsole[] consoles = FindObjectsOfType<OVRLipSyncDebugConsole>();
        if (consoles.Length > 0)
        {
            hasDebugConsole = consoles[0];
        }
    }

    /// <summary>
    /// Handle keyboard input
    /// </summary>
    void HandleKeyboard()
    {
        // Turn loopback on/off
        if (Input.GetKeyDown(loopbackKey))
        {
            ToggleAudioLoopback();
        }
        else if (Input.GetKeyDown(debugVisemesKey))
        {
            showVisemes = !showVisemes;

            if (showVisemes)
            {
                if (hasDebugConsole)
                {
                    Debug.Log("DEBUG SHOW VISEMES: ENABLED");
                }
                else
                {
                    Debug.LogWarning("Warning: No OVRLipSyncDebugConsole in the scene!");
                    showVisemes = false;
                }
            }
            else
            {
                if (hasDebugConsole)
                {
                    OVRLipSyncDebugConsole.Clear();
                }
                Debug.Log("DEBUG SHOW VISEMES: DISABLED");
            }
        }
        else if (Input.GetKeyDown(debugLaughterKey))
        {
            showLaughter = !showLaughter;

            if (showLaughter)
            {
                if (hasDebugConsole)
                {
                    Debug.Log("DEBUG SHOW LAUGHTER: ENABLED");
                }
                else
                {
                    Debug.LogWarning("Warning: No OVRLipSyncDebugConsole in the scene!");
                    showLaughter = false;
                }
            }
            else
            {
                if (hasDebugConsole)
                {
                    OVRLipSyncDebugConsole.Clear();
                }
                Debug.Log("DEBUG SHOW LAUGHTER: DISABLED");
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gain -= 1.0f;
            if (gain < 1.0f) gain = 1.0f;

            string g = "LINEAR GAIN: ";
            g += gain;

            if (hasDebugConsole)
            {
                OVRLipSyncDebugConsole.Clear();
                OVRLipSyncDebugConsole.Log(g);
                OVRLipSyncDebugConsole.ClearTimeout(1.5f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            gain += 1.0f;
            if (gain > 15.0f)
                gain = 15.0f;

            string g = "LINEAR GAIN: ";
            g += gain;

            if (hasDebugConsole)
            {
                OVRLipSyncDebugConsole.Clear();
                OVRLipSyncDebugConsole.Log(g);
                OVRLipSyncDebugConsole.ClearTimeout(1.5f);
            }
        }
    }

    /// <summary>
    /// Run processes that need to be updated in our game thread
    /// </summary>
    void Update()
    {
        if (enableKeyboardInput)
        {
            HandleKeyboard();
        }
        laughterScore = this.Frame.laughterScore;
        DebugShowVisemesAndLaughter();
    }

    /// <summary>
    /// Preprocess F32 PCM audio buffer
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="channels">Channels.</param>
    public void PreprocessAudioSamples(float[] data, int channels)
    {
        // Increase the gain of the input
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = data[i] * gain;
        }
    }

    /// <summary>
    /// Postprocess F32 PCM audio buffer
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="channels">Channels.</param>
    public void PostprocessAudioSamples(float[] data, int channels)
    {
        // Turn off output (so that we don't get feedback from mics too close to speakers)
        if (!audioLoopback)
        {
            for (int i = 0; i < data.Length; ++i)
                data[i] = data[i] * 0.0f;
        }
    }

    /// <summary>
    /// Pass F32 PCM audio buffer to the lip sync module
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="channels">Channels.</param>
    public void ProcessAudioSamplesRaw(float[] data, int channels)
    {
        // BUG: Locking the 'this' object in a lock statement
        // MESSAGE: It is bad practice to lock the 'this' object because it might be locked elsewhere.
        //         // Send data into Phoneme context for processing (if context is not 0)
        //         lock (this)
        //         {
        //             if (Context == 0 || OVRLipSync.IsInitialized() != OVRLipSync.Result.Success)
        //             {
        //                 return;
        //             }
        //             var frame = this.Frame;
        //             OVRLipSync.ProcessFrame(Context, data, frame, channels == 2);
        //         }
        //     }

        // FIXED VERSION:
