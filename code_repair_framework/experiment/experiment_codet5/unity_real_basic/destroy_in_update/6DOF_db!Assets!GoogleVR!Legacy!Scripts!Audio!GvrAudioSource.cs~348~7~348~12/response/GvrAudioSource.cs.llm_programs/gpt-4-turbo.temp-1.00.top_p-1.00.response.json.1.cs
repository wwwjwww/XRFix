













using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

#pragma warning disable 0618 // Ignore GvrAudio* deprecation


#if UNITY_2017_1_OR_NEWER
[System.Obsolete("Please upgrade to Resonance Audio (https://developers.google.com/resonance-audio/migrate).")]
#endif  // UNITY_2017_1_OR_NEWER
[AddComponentMenu("GoogleVR/Audio/GvrAudioSource")]
public class GvrAudioSource : MonoBehaviour {

  public bool bypassRoomEffects = false;


  public float directivityAlpha = 0.0f;


  public float directivitySharpness = 1.0f;


  public float listenerDirectivityAlpha = 0.0f;


  public float listenerDirectivitySharpness = 1.0f;


  public float gainDb = 0.0f;


  public bool occlusionEnabled = false;


  public bool playOnAwake = true;


  public AudioClip clip {
    get { return sourceClip; }
    set {
      sourceClip = value;
      if (audioSource != null) {
        audioSource.clip = sourceClip;
      }
    }
  }
  [SerializeField]
  private AudioClip sourceClip = null;


  public bool isPlaying {
    get {
      if (audioSource != null) {
        return audioSource.isPlaying;
      }
      return false;
    }
  }


  public bool loop {
    get { return sourceLoop; }
    set {
      sourceLoop = value;
      if (audioSource != null) {
        audioSource.loop = sourceLoop;
      }
    }
  }
  [SerializeField]
  private bool sourceLoop = false;


  public bool mute {
    get { return sourceMute; }
    set {
      sourceMute = value;
      if (audioSource != null) {
        audioSource.mute = sourceMute;
      }
    }
  }
  [SerializeField]
  private bool sourceMute = false;


  public float pitch {
    get { return sourcePitch; }
    set {
      sourcePitch = value;
      if (audioSource != null) {
        audioSource.pitch = sourcePitch;
      }
    }
  }
  [SerializeField]
  [Range(-3.0f, 3.0f)]
  private float sourcePitch = 1.0f;


  public int priority {
    get { return sourcePriority; }
    set {
      sourcePriority = value;
      if(audioSource != null) {
        audioSource.priority = sourcePriority;
      }
    }
  }
  [SerializeField]
  [Range(0, 256)]
  private int sourcePriority = 128;


  public float spatialBlend {
    get { return sourceSpatialBlend; }
    set {
      sourceSpatialBlend = value;
      if (audioSource != null) {
        audioSource.spatialBlend = sourceSpatialBlend;
      }
    }
  }
  [SerializeField]
  [Range(0.0f, 1.0f)]
  private float sourceSpatialBlend = 1.0f;


  public float dopplerLevel {
    get { return sourceDopplerLevel; }
    set {
      sourceDopplerLevel = value;
      if(audioSource != null) {
        audioSource.dopplerLevel = sourceDopplerLevel;
      }
    }
  }
  [SerializeField]
  [Range(0.0f, 5.0f)]
  private float sourceDopplerLevel = 1.0f;


  public float spread {
    get { return sourceSpread; }
    set {
      sourceSpread = value;
      if(audioSource != null) {
        audioSource.spread = sourceSpread;
      }
    }
  }
  [SerializeField]
  [Range(0.0f, 360.0f)]
  private float sourceSpread = 0.0f;


  public float time {
    get {
      if(audioSource != null) {
        return audioSource.time;
      }
      return 0.0f;
    }
    set {
      if(audioSource != null) {
        audioSource.time = value;
      }
    }
  }


  public int timeSamples {
    get {
      if(audioSource != null) {
        return audioSource.timeSamples;
      }
      return 0;
    }
    set {
      if(audioSource != null) {
        audioSource.timeSamples = value;
      }
    }
  }


  public float volume {
    get { return sourceVolume; }
    set {
      sourceVolume = value;
      if (audioSource != null) {
        audioSource.volume = sourceVolume;
      }
    }
  }
  [SerializeField]
  [Range(0.0f, 1.0f)]
  private float sourceVolume = 1.0f;


  public AudioRolloffMode rolloffMode {
    get { return sourceRolloffMode; }
    set {
      sourceRolloffMode = value;
      if (audioSource != null) {
        audioSource.rolloffMode = sourceRolloffMode;
        if (rolloffMode == AudioRolloffMode.Custom) {

          audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff,
                                     AnimationCurve.Linear(sourceMinDistance, 1.0f,
                                                           sourceMaxDistance, 1.0f));
        }
      }
    }
  }
  [SerializeField]
  private AudioRolloffMode sourceRolloffMode = AudioRolloffMode.Logarithmic;


  public float maxDistance {
    get { return sourceMaxDistance; }
    set {
      sourceMaxDistance = Mathf.Clamp(value, sourceMinDistance + GvrAudio.distanceEpsilon,
                                      GvrAudio.maxDistanceLimit);
      if(audioSource != null) {
        audioSource.maxDistance = sourceMaxDistance;
      }
    }
  }
  [SerializeField]
  private float sourceMaxDistance = 500.0f;


  public float minDistance {
    get { return sourceMinDistance; }
    set {
      sourceMinDistance = Mathf.Clamp(value, 0.0f, GvrAudio.minDistanceLimit);
      if(audioSource != null) {
        audioSource.minDistance = sourceMinDistance;
      }
    }
  }
  [SerializeField]
  private float sourceMinDistance = 1.0f;


  [SerializeField]
  private bool hrtfEnabled = true;


  [SerializeField]
  private AudioSource audioSource = null;


  private int id = -1;


  private float currentOcclusion = 0.0f;


  private float nextOcclusionUpdate = 0.0f;


  private bool isPaused = false;

  void Awake () {
#if UNITY_EDITOR && UNITY_2017_1_OR_NEWER
    Debug.LogWarningFormat(gameObject,
        "Game object '{0}' uses deprecated {1} component.\nPlease upgrade to Resonance Audio ({2}).",
        name, GetType().Name, "https://developers.google.com/resonance-audio/migrate");
#endif  // UNITY_EDITOR && UNITY_2017_1_OR_NEWER
    if (audioSource == null) {

      audioSource = gameObject.AddComponent<AudioSource>();
    }
    audioSource.enabled = false;
    audioSource.hideFlags = HideFlags.HideInInspector | HideFlags.HideAndDontSave;
    audioSource.playOnAwake = false;
    audioSource.bypassReverbZones = true;
#if UNITY_5_5_OR_NEWER
    audioSource.spatializePostEffects = true;
#endif  // UNITY_5_5_OR_NEWER
    OnValidate();

    AudioMixer mixer = (Resources.Load("GvrAudioMixer") as AudioMixer);
    if(mixer != null) {
      audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
    } else {
      Debug.LogError("GVRAudioMixer could not be found in Resources. Make sure that the GVR SDK " +
                     "Unity package is imported properly.");
    }
  }

  void OnEnable () {
    audioSource.enabled = true;
    if (playOnAwake && !isPlaying && InitializeSource()) {
      Play();
    }
  }

  void Start () {
    if (playOnAwake && !isPlaying) {
      Play();
    }
  }

  void OnDisable () {
    Stop();
    audioSource.enabled = false;
  }

  void OnDestroy () {
    Destroy(audioSource);
  }

  void OnApplicationPause (bool pauseStatus) {
    if (pauseStatus) {
      Pause();
    } else {
      UnPause();
    }
  }


// BUG: Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
//  void Update () {
//
//    if (!occlusionEnabled) {
//      currentOcclusion = 0.0f;
//    } else if (Time.time >= nextOcclusionUpdate) {
//      nextOcclusionUpdate = Time.time + GvrAudio.occlusionDetectionInterval;
//      currentOcclusion = GvrAudio.ComputeOcclusion(transform);
//    }
//
//    if (!isPlaying && !isPaused) {
//      Stop();
//    } else {
//      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Gain,
//                                      GvrAudio.ConvertAmplitudeFromDb(gainDb));
//      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.MinDistance,
//                                      sourceMinDistance);
//      GvrAudio.UpdateAudioSource(id, this, currentOcclusion);
//    }
//  }





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





void Update()
{
    if (!occlusionEnabled)
    {
        currentOcclusion = 0.0f;
    }
    else if (Time.time >= nextOcclusionUpdate)
    {
        nextOcclusionUpdate = Time.time + GvrAudio.occlusionDetectionInterval;
        currentOcclusion = GvrAudio.ComputeOcclusion(transform);
    }

    if (!isPlaying && !isPaused)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            ShutdownSource();
            isPaused = true;
        }
    }
    else
    {
        if (audioSource != null)
        {
            audioSource.SetSpatializerFloat((int)GvrAudio.SpatializerData.Gain,
                GvrAudio.ConvertAmplitudeFromDb(gainDb));
            audioSource.SetSpatializerFloat((int)GvrAudio.SpatializerData.MinDistance,
                sourceMinDistance);
            GvrAudio.UpdateAudioSource(id, this, currentOcclusion);
        }
    }
}

private void ShutdownSource()
{
    if (id >= 0)
    {
        if (audioSource != null)
        {
            audioSource.SetSpatializerFloat((int)GvrAudio.SpatializerData.Id, -1.0f);
            audioSource.SetSpatializerFloat((int)GvrAudio.SpatializerData.ZeroOutput, 1.0f);
            audioSource.spatialize = false;
        }
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
