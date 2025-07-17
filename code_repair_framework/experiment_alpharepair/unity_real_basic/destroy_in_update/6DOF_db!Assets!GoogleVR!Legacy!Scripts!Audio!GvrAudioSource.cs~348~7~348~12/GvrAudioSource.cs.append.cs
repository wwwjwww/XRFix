
  /// Provides a block of the currently playing source's output data.
  ///
  /// @note The array given in samples will be filled with the requested data before spatialization.
  public void GetOutputData(float[] samples, int channel) {
    if (audioSource != null) {
      audioSource.GetOutputData(samples, channel);
    }
  }

  /// Provides a block of the currently playing audio source's spectrum data.
  ///
  /// @note The array given in samples will be filled with the requested data before spatialization.
  public void GetSpectrumData(float[] samples, int channel, FFTWindow window) {
    if (audioSource != null) {
      audioSource.GetSpectrumData(samples, channel, window);
    }
  }

  /// Pauses playing the clip.
  public void Pause () {
    if (audioSource != null) {
      isPaused = true;
      audioSource.Pause();
    }
  }

  /// Plays the clip.
  public void Play () {
    if (audioSource != null && InitializeSource()) {
      audioSource.Play();
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  /// Plays the clip with a delay specified in seconds.
  public void PlayDelayed (float delay) {
    if (audioSource != null && InitializeSource()) {
      audioSource.PlayDelayed(delay);
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  /// Plays an AudioClip.
  public void PlayOneShot (AudioClip clip) {
    PlayOneShot(clip, 1.0f);
  }

  /// Plays an AudioClip, and scales its volume.
  public void PlayOneShot (AudioClip clip, float volume) {
    if (audioSource != null && InitializeSource()) {
      audioSource.PlayOneShot(clip, volume);
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  /// Plays the clip at a specific time on the absolute time-line that AudioSettings.dspTime reads
  /// from.
  public void PlayScheduled (double time) {
    if (audioSource != null && InitializeSource()) {
      audioSource.PlayScheduled(time);
      isPaused = false;
    } else {
      Debug.LogWarning ("GVR Audio source not initialized. Audio playback not supported " +
                        "until after Awake() and OnEnable(). Try calling from Start() instead.");
    }
  }

  /// Changes the time at which a sound that has already been scheduled to play will end.
  public void SetScheduledEndTime(double time) {
    if (audioSource != null) {
      audioSource.SetScheduledEndTime(time);
    }
  }

  /// Changes the time at which a sound that has already been scheduled to play will start.
  public void SetScheduledStartTime(double time) {
    if (audioSource != null) {
      audioSource.SetScheduledStartTime(time);
    }
  }

  /// Stops playing the clip.
  public void Stop () {
    if (audioSource != null) {
      audioSource.Stop();
      ShutdownSource();
      isPaused = true;
    }
  }

  /// Unpauses the paused playback.
  public void UnPause () {
    if (audioSource != null) {
      audioSource.UnPause();
      isPaused = false;
    }
  }

  // Initializes the source.
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
        // Source id must be set after all the spatializer parameters, to ensure that the source is
        // properly initialized before processing.
        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Id, (float) id);
      }
    }
    return id >= 0;
  }

  // Shuts down the source.
  private void ShutdownSource () {
    if (id >= 0) {
      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Id, -1.0f);
      // Ensure that the output is zeroed after shutdown.
      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.ZeroOutput, 1.0f);
      audioSource.spatialize = false;
      GvrAudio.DestroyAudioSource(id);
      id = -1;
    }
  }

  void OnDidApplyAnimationProperties () {
    OnValidate();
  }

  void OnValidate () {
    clip = sourceClip;
    loop = sourceLoop;
    mute = sourceMute;
    pitch = sourcePitch;
    priority = sourcePriority;
    spatialBlend = sourceSpatialBlend;
    volume = sourceVolume;
    dopplerLevel = sourceDopplerLevel;
    spread = sourceSpread;
    minDistance = sourceMinDistance;
    maxDistance = sourceMaxDistance;
    rolloffMode = sourceRolloffMode;
  }

  void OnDrawGizmosSelected () {
    // Draw listener directivity gizmo.
    // Note that this is a very suboptimal way of finding the component, to be used in Unity Editor
    // only, should not be used to access the component in run time.
    GvrAudioListener listener = FindObjectOfType<GvrAudioListener>();
    if(listener != null) {
      Gizmos.color = GvrAudio.listenerDirectivityColor;
      DrawDirectivityGizmo(listener.transform, listenerDirectivityAlpha,
                           listenerDirectivitySharpness, 180);
    }
    // Draw source directivity gizmo.
    Gizmos.color = GvrAudio.sourceDirectivityColor;
    DrawDirectivityGizmo(transform, directivityAlpha, directivitySharpness, 180);
  }

  // Draws a 3D gizmo in the Scene View that shows the selected directivity pattern.
  private void DrawDirectivityGizmo (Transform target, float alpha, float sharpness,
                                     int resolution) {
    Vector2[] points = GvrAudio.Generate2dPolarPattern(alpha, sharpness, resolution);
    // Compute |vertices| from the polar pattern |points|.
    int numVertices = resolution + 1;
    Vector3[] vertices = new Vector3[numVertices];
    vertices[0] = Vector3.zero;
    for (int i = 0; i < points.Length; ++i) {
      vertices[i + 1] = new Vector3(points[i].x, 0.0f, points[i].y);
    }
    // Generate |triangles| from |vertices|. Two triangles per each sweep to avoid backface culling.
    int[] triangles = new int[6 * numVertices];
    for (int i = 0; i < numVertices - 1; ++i) {
      int index = 6 * i;
      if (i < numVertices - 2) {
        triangles[index] = 0;
        triangles[index + 1] = i + 1;
        triangles[index + 2] = i + 2;
      } else {
        // Last vertex is connected back to the first for the last triangle.
        triangles[index] = 0;
        triangles[index + 1] = numVertices - 1;
        triangles[index + 2] = 1;
      }
      // The second triangle facing the opposite direction.
      triangles[index + 3] = triangles[index];
      triangles[index + 4] = triangles[index + 2];
      triangles[index + 5] = triangles[index + 1];
    }
    // Construct a new mesh for the gizmo.
    Mesh directivityGizmoMesh = new Mesh();
    directivityGizmoMesh.hideFlags = HideFlags.DontSaveInEditor;
    directivityGizmoMesh.vertices = vertices;
    directivityGizmoMesh.triangles = triangles;
    directivityGizmoMesh.RecalculateNormals();
    // Draw the mesh.
    Vector3 scale = 2.0f * Mathf.Max(target.lossyScale.x, target.lossyScale.z) * Vector3.one;
    Gizmos.DrawMesh(directivityGizmoMesh, target.position, target.rotation, scale);
  }
}

#pragma warning restore 0618 // Restore warnings
