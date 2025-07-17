
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
    private uint context = 0;    

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
    private uint context = 0;

    private int _smoothing;
    public int Smoothing
    {
        set
        {
            OVRLipSync.Result result = OVRLipSync.SendSignal(context, OVRLipSync.Signals.VisemeSmoothing, value, 0);

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

    private void Start()
    {
        context = OVRLipSync.CreateContext(audioSource.clip, frame);

        if (provider != OVRLipSync.ContextProviders.Enhanced)
        {
            Debug.LogWarning("OVRLipSyncContextBase: The selected lip sync provider is not compatible" +
                " with the current Unity version. Using Enhanced provider instead.");
            provider = OVRLipSync.ContextProviders.Enhanced;
        }
    }

    private void FixedUpdate()
    {
        OVRLipSync.Result result = OVRLipSync.UpdateContext(context, provider, frame, enableAcceleration);

        if (result != OVRLipSync.Result.Success)
        {
            Debug.LogError("OVRLipSyncContextBase: An unexpected error occurred during lip sync computation.");
        }
    }
}



    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    void OnDestroy()
    {
        // Create the context that we will feed into the audio buffer
        lock (this)
        {
            if (context != 0)
            {
                if (OVRLipSync.DestroyContext(context) != OVRLipSync.Result.Success)
                {
                    Debug.LogError("OVRLipSyncContextBase.OnDestroy ERROR: Could not delete" +
                        " Phoneme context.");
                }
            }
        }
    }

    // * * * * * * * * * * * * *
    // Public Functions

    /// <summary>
    /// Gets the current phoneme frame (lock and copy current frame to caller frame)
    /// </summary>
    /// <returns>error code</returns>
    /// <param name="inFrame">In frame.</param>
    public OVRLipSync.Frame GetCurrentPhonemeFrame()
    {
        return frame;
    }

    /// <summary>
    /// Sets a given viseme id blend weight to a given amount
    /// </summary>
    /// <param name="viseme">Integer viseme ID</param>
    /// <param name="amount">Integer viseme amount</param>
    public void SetVisemeBlend(int viseme, int amount)
    {
        OVRLipSync.Result result =
            OVRLipSync.SendSignal(context, OVRLipSync.Signals.VisemeAmount, viseme, amount);

        if (result != OVRLipSync.Result.Success)
        {
            if (result == OVRLipSync.Result.InvalidParam)
            {
                Debug.LogError("OVRLipSyncContextBase.SetVisemeBlend: Viseme ID is invalid.");
            }
            else
            {
                Debug.LogError("OVRLipSyncContextBase.SetVisemeBlend: An unexpected" +
                    " error occured.");
            }
        }
    }

    /// <summary>
    /// Sets a given viseme id blend weight to a given amount
    /// </summary>
    /// <param name="amount">Integer viseme amount</param>
    public void SetLaughterBlend(int amount)
    {
        OVRLipSync.Result result =
            OVRLipSync.SendSignal(context, OVRLipSync.Signals.LaughterAmount, amount, 0);

        if (result != OVRLipSync.Result.Success)
        {
            Debug.LogError("OVRLipSyncContextBase.SetLaughterBlend: An unexpected" +
                " error occured.");
        }
    }

    /// <summary>
    /// Resets the context.
    /// </summary>
    /// <returns>error code</returns>
    public OVRLipSync.Result ResetContext()
    {
        // Reset visemes to silence etc.
        frame.Reset();

        return OVRLipSync.ResetContext(context);
    }
}
