

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
