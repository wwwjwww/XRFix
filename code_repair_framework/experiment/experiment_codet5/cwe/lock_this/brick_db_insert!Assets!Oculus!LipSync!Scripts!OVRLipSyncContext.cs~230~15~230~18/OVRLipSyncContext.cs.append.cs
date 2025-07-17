
    /// <summary>
    /// Pass S16 PCM audio buffer to the lip sync module
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="channels">Channels.</param>
    public void ProcessAudioSamplesRaw(short[] data, int channels)
    {
        // Send data into Phoneme context for processing (if context is not 0)
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
