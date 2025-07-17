
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

    
    
    
    void Awake()
    {
        
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

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

        // Create a private readonly Object which is used exclusively for locking. This ensures that no other classes can use the same lock.
        // FIXED CODE:

        }
using UnityEngine;

// This Object is used exclusively for locking. No other classes should use it!
private static readonly Object _lock = new Object();

public class OVRLipSyncContextBase : MonoBehaviour
{
    // Declare a new AudioSource field
    public AudioSource audioSource;

    // Declare a new provider field of type OVRLipSync.ContextProviders
    public OVRLipSync.ContextProviders provider;

    // Declare a new enableAcceleration field of type boolean
    public bool enableAcceleration;

    // Declare a new private field of type OVRLipSync.Frame
    private OVRLipSync.Frame frame;

    // Declare a new private field of type uint, which will store the context
    private uint context;

    // Declare a new public property of type int, which will store the smoothing value
    public int Smoothing
    {
        set
        {
            // Use a lock statement to ensure that only one thread accesses the context at a time
            lock (_lock)
            {
                // Check if the context is valid
                if (context!= 0)
                {
                    // Use the OVRLipSync.SendSignal function to set the smoothing value on the context
                    OVRLipSync.Result result = OVRLipSync.SendSignal(context, OVRLipSync.Signals.VisemeSmoothing, value, 0);

                    // If the result is not successful, print an error message
                    if (result!= OVRLipSync.Result.Success)
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
                }

                // Set the smoothing value to the new value
                _smoothing = value;
            }
        }
        get
        {
            return _smoothing;
        }
    }

    // Declare a new public property of type uint, which will return the context
    public uint Context
    {
        get
        {
            return context;
        }
    }

    // Declare a new public property of type OVRLipSync.Frame, which will return the frame
    public OVRLipSync.Frame Frame
    {
        get
        {
            return frame;
        }
    }

    // Awake is called before any of the script's functions are called
    void Awake()
    {
        // Check if the AudioSource component is present on the game object
        if (audioSource == null)
        {
            // If the AudioSource component is not present, add it
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Check if the context is zero
        if (context == 0)
        {
            // Use a lock statement to ensure that only one thread creates the context at a time
            lock (_lock)
            {
                // Check if the context is still zero
                if (context == 0)
                {
                    // Use the OVRLipSync.CreateContext function to create the context
                    OVRLipSync.Result result = OVRLipSync.CreateContext(ref context, provider, 0, enableAcceleration);

                    // If the result


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
