
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

[RequireComponent(typeof(AudioSource))]

public class OVRLipSyncContextBase : MonoBehaviour
{
	private AudioSource audioSource;

	public enum ContextProviders
	{
		Enhanced,
		Basic,
		None
	}

	public ContextProviders provider = ContextProviders.Enhanced;
	public bool enableAcceleration = true;

	private OVRLipSync.Frame frame = new OVRLipSync.Frame();
	private uint context;

	private int smoothing;

	public uint Context
	{
		get
		{
			return context;
		}
	}

	public OVRLipSync.Frame Frame
	{
		get
		{
			return frame;
		}
	}

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();

		if (context == 0)
		{
			CreateContext();
		}
	}

	void OnDestroy()
	{
		OVRLipSync.DestroyContext(context);
	}

	void Update()
	{
		int sampleCount = audioSource.clip.samples;
		float[] samples = new float[sampleCount];
		audioSource.GetOutputData(samples, 0);

		OVRLipSync.Result result = OVRLipSync.ProcessAudio(context, samples, (uint)sampleCount);
		if (result!= OVRLipSync.Result.Success)
		{
			Debug.LogError("OVRLipSyncContextBase.Update: An error occurred while processing audio: " + result);
			return;
		}

		bool updated = OVRLipSync.GetFrame(context, ref frame);
		if (!updated)
		{
			Debug.LogError("OVRLipSyncContextBase.Update: An error occurred while getting frame.");
			return;
		}
	}

	private void CreateContext()
	{
		OVRLipSync.Result result = OVRLipSync.CreateContext(ref context, (OVRLipSync.ContextProviders)provider, 0, enableAcceleration);
		if (result!= OVRLipSync.Result.Success)
		{
			Debug.LogError("OVRLipSyncContextBase.CreateContext: Could not create Lip Sync context: " + result);
			return;
		}

		SetSmoothing(smoothing);
	}

	public void SetSmoothing(int value)
	{
		if (value < 1 || value > 100)
		{
			Debug.LogError("OVRLipSyncContextBase.SetSmoothing: A viseme smoothing parameter is invalid. It should be between 1 and 100.");
			return;
		}

		OVRLipSync.Result result = OVRLipSync.SendSignal(context, OVRLipSync.Signals.VisemeSmoothing, value, 0);
		if (result!= OVRLipSync.Result.Success)
		{
			Debug.LogError("OVRLipSyncContextBase.SetSmoothing: An unexpected error occurred.");
			return;
		}

		smoothing = value;
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
