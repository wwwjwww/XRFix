
  
  
  
  public void GetOutputData(float[] samples, int channel) {
    if (audioSource != null) {
      audioSource.GetOutputData(samples, channel);
    }
  }

  
  
  
  public void GetSpectrumData(float[] samples, int channel, FFTWindow window) {
    if (audioSource != null) {
      audioSource.GetSpectrumData(samples, channel, window);
    }
  }

  
  public void Pause () {
    if (audioSource != null) {
      isPaused = true;
      audioSource.Pause();
    }
  }

  
  public void Play () {
    if (audioSource != null && InitializeSource()) {
      audioSource.Play();
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  
  public void PlayDelayed (float delay) {
    if (audioSource != null && InitializeSource()) {
      audioSource.PlayDelayed(delay);
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  
  public void PlayOneShot (AudioClip clip) {
    PlayOneShot(clip, 1.0f);
  }

  
  public void PlayOneShot (AudioClip clip, float volume) {
    if (audioSource != null && InitializeSource()) {
      audioSource.PlayOneShot(clip, volume);
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  
  
  public void PlayScheduled (double time) {
    if (audioSource != null && InitializeSource()) {
      audioSource.PlayScheduled(time);
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  
  public void SetScheduledEndTime(double time) {
    if (audioSource != null) {
      audioSource.SetScheduledEndTime(time);
    }
  }

  
  public void SetScheduledStartTime(double time) {
    if (audioSource != null) {
      audioSource.SetScheduledStartTime(time);
    }
  }

  
  public void Stop () {
    if (audioSource != null) {
      audioSource.Stop();
      ShutdownSource();
      isPaused = true;
    }
  }

  
  public void UnPause () {
    if (audioSource != null) {
      audioSource.UnPause();
      isPaused = false;
    }
  }

  
  private bool InitializeSource () {
    if (id < 0) {
      id = GvrAudio.CreateAudioSource(hrtfEnabled);
      if (id >= 0) {
        GvrAudio.UpdateAudioSource(id, this, currentOcclusion);
        audioSource.spatialize = true;
        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Type,
                                        (float) GvrAudio.SpatializerType.Source);
        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Gain,
                                        GvrAudio.ConvertAmplitudeFromDb(gainDb));
        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.MinDistance,
                                        sourceMinDistance);
        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.ZeroOutput, 0.0f);
        
        
        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Id, (float) id);
      }
    }
    return id >= 0;
  }

  
