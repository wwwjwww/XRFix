
using UnityEngine;

[RequireComponent(typeof(AudioSource))]









public class OVRLipSyncContext : OVRLipSyncContextBase
{




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








    void Start()
    {

        if (enableTouchInput)
        {
            OVRTouchpad.AddListener(LocalTouchEventCallback);
        }


        OVRLipSyncDebugConsole[] consoles = FindObjectsOfType<OVRLipSyncDebugConsole>();
        if (consoles.Length > 0)
        {
            hasDebugConsole = consoles[0];
        }
    }




    void HandleKeyboard()
    {

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




    void Update()
    {
        if (enableKeyboardInput)
        {
            HandleKeyboard();
        }
        laughterScore = this.Frame.laughterScore;
        DebugShowVisemesAndLaughter();
    }






    public void PreprocessAudioSamples(float[] data, int channels)
    {

        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = data[i] * gain;
        }
    }






    public void PostprocessAudioSamples(float[] data, int channels)
    {

        if (!audioLoopback)
        {
            for (int i = 0; i < data.Length; ++i)
                data[i] = data[i] * 0.0f;
        }
    }






    public void ProcessAudioSamplesRaw(float[] data, int channels)
    {

        lock (this)
        {
            if (Context == 0 || OVRLipSync.IsInitialized() != OVRLipSync.Result.Success)
            {
                return;
            }
            var frame = this.Frame;
            OVRLipSync.ProcessFrame(Context, data, frame, channels == 2);
        }
    }






///     public void ProcessAudioSamplesRaw(short[] data, int channels)
//     {
// 
        // BUG: Locking the 'this' object in a lock statement
        // MESSAGE: It is bad practice to lock the 'this' object because it might be locked elsewhere.
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

        // Create a private readonly Object which is used exclusively for locking. This ensures that no other classes can use the same lock.
        // FIXED CODE:


private readonly object lockObject = new object();

public new void ProcessAudioSamplesRaw(short[] data, int channels)
{
    lock (lockObject)
    {
        if (Context == 0 || OVRLipSync.IsInitialized() != OVRLipSync.Result.Success)
        {
            return;
        }
        var frame = this.Frame;
        OVRLipSync.ProcessFrame(Context, data, frame, channels == 2);
    }
}




    /// <summary>
    /// Process F32 audio sample and pass it to the lip sync module for computation
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="channels">Channels.</param>
    public void ProcessAudioSamples(float[] data, int channels)
    {
        // Do not process if we are not initialized, or if there is no
        // audio source attached to game object
        if ((OVRLipSync.IsInitialized() != OVRLipSync.Result.Success) || audioSource == null)
        {
            return;
        }
        PreprocessAudioSamples(data, channels);
        ProcessAudioSamplesRaw(data, channels);
        PostprocessAudioSamples(data, channels);
    }

    /// <summary>
    /// Raises the audio filter read event.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="channels">Channels.</param>
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!skipAudioSource)
        {
            ProcessAudioSamples(data, channels);
        }
    }

    /// <summary>
    /// Print the visemes and laughter score to game window
    /// </summary>
    void DebugShowVisemesAndLaughter()
    {
        if (hasDebugConsole)
        {
            string seq = "";
            if (showLaughter)
            {
                seq += "Laughter:";
                int count = (int)(50.0f * this.Frame.laughterScore);
                for (int c = 0; c < count; c++)
                    seq += "*";
                seq += "\n";
            }
            if (showVisemes)
            {
                for (int i = 0; i < this.Frame.Visemes.Length; i++)
                {
                    seq += ((OVRLipSync.Viseme)i).ToString();
                    seq += ":";

                    int count = (int)(50.0f * this.Frame.Visemes[i]);
                    for (int c = 0; c < count; c++)
                        seq += "*";

                    seq += "\n";
                }
            }

            OVRLipSyncDebugConsole.Clear();

            if (seq != "")
            {
                OVRLipSyncDebugConsole.Log(seq);
            }
        }
    }

    void ToggleAudioLoopback()
    {
        audioLoopback = !audioLoopback;

        if (hasDebugConsole)
        {
            OVRLipSyncDebugConsole.Clear();
            OVRLipSyncDebugConsole.ClearTimeout(1.5f);

            if (audioLoopback)
                OVRLipSyncDebugConsole.Log("LOOPBACK MODE: ENABLED");
            else
                OVRLipSyncDebugConsole.Log("LOOPBACK MODE: DISABLED");
        }
    }

    // LocalTouchEventCallback
    void LocalTouchEventCallback(OVRTouchpad.TouchEvent touchEvent)
    {
        string g = "LINEAR GAIN: ";

        switch (touchEvent)
        {
            case (OVRTouchpad.TouchEvent.SingleTap):
                ToggleAudioLoopback();
                break;

            case (OVRTouchpad.TouchEvent.Up):
                gain += 1.0f;
                if (gain > 15.0f)
                    gain = 15.0f;

                g += gain;

                if (hasDebugConsole)
                {
                    OVRLipSyncDebugConsole.Clear();
                    OVRLipSyncDebugConsole.Log(g);
                    OVRLipSyncDebugConsole.ClearTimeout(1.5f);
                }

                break;

            case (OVRTouchpad.TouchEvent.Down):
                gain -= 1.0f;
                if (gain < 1.0f) gain = 1.0f;

                g += gain;

                if (hasDebugConsole)
                {
                    OVRLipSyncDebugConsole.Clear();
                    OVRLipSyncDebugConsole.Log(g);
                    OVRLipSyncDebugConsole.ClearTimeout(1.5f);
                }

                break;
        }
    }
}
