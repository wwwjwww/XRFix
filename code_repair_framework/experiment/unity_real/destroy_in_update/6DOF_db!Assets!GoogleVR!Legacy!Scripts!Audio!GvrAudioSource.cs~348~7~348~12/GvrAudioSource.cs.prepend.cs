













using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

#pragma warning disable 0618 


#if UNITY_2017_1_OR_NEWER
[System.Obsolete("Please upgrade to Resonance Audio (https:
#endif  
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
        name, GetType().Name, "https:
#endif  
    if (audioSource == null) {
      
      audioSource = gameObject.AddComponent<AudioSource>();
    }
    audioSource.enabled = false;
    audioSource.hideFlags = HideFlags.HideInInspector | HideFlags.HideAndDontSave;
    audioSource.playOnAwake = false;
    audioSource.bypassReverbZones = true;
#if UNITY_5_5_OR_NEWER
    audioSource.spatializePostEffects = true;
#endif  
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

