using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using BaroqueUI;
using System;


namespace NanIndustryVR
{
    public class SceneLoader : MonoBehaviour
    {
        public GamePrefabs gamePrefabs;
        public bool introScreen;
        public bool loadingScreen;
        public AudioClip backgroundMusic;
        public float backgroundMusicVolume;


        public static SceneLoader scene_loader;
        public static GamePrefabs game_prefabs;
        public static World world;
        public static ToolSelectionUI tsel_ui;
        public static ScalerRotater scaler_rotater;

        protected Rigidbody rb3;



        private void Awake()
        {
            scene_loader = this;

            if (game_prefabs == null)
            {
                ScreenLog.Initialize();

                game_prefabs = gamePrefabs;
                game_prefabs.Initialize();

                OneShotEvent(Shader.WarmupAllShaders);
            }

            //Instantiate(game_prefabs.cameraRig);
            music_volume_forced_reduced = false;
        }

        private void Start()
        {
            if (loadingScreen)
            {
                SceneManager.LoadScene("Intro");
                return;
            }
            game_prefabs.customizeController.StartControllers();

            var gt = Controller.GlobalTracker(this);
            gt.onControllersUpdate += Gt_onControllersUpdate;

            OneShotEvent(CheckBackgroundMusic);

#if UNITY_EDITOR && false
            Controller.menu_button_for_screenshot = true;
            if (!loadingScreen && !introScreen)
            {
                var name = SceneManager.GetActiveScene().name;
                Debug.Assert(name.StartsWith("Bkgnd "));
                name = name.Substring("Bkgnd ".Length);
                var index = gamePrefabs.skyboxNames.ToList().IndexOf(name);
                Debug.Assert(index >= 0);
                Debug.Assert(gamePrefabs.skyboxMaterials[index] == RenderSettings.skybox);
            }
#endif
        }

        void Gt_onControllersUpdate(Controller[] controllers)
        {
            if (scaler_rotater)
                scaler_rotater.ControllersUpdate(controllers);

            if (tsel_ui)
                tsel_ui.ControllersUpdate(controllers);
        }

        public static void AddScalerRotater()
        {
            if (!scaler_rotater)
                scaler_rotater = new GameObject("Scaler Rotater").AddComponent<ScalerRotater>();
        }

        public static void AddToolSelectionUI(ToolSelectionUI tsel_ui_prefab, bool deselect = false)
        {
            if (tsel_ui)
            {
                Destroy(tsel_ui.gameObject);
                tsel_ui = null;
                OneShotEvent(() => AddToolSelectionUI(tsel_ui_prefab, deselect));
            }
            else
            {
                tsel_ui = Instantiate(tsel_ui_prefab);
                tsel_ui.deselect_any_tool = deselect;
            }
        }


        /********** Background music **********/

        /* only null at the very beginning of the game, but may later change or have its volume
         * turned to 0 */
        static AudioSource global_audio_source;
        static float global_audio_volume, global_audio_volume_max;
        static bool music_volume_forced_reduced;

        static void MakeGlobalAudioSource(AudioClip clip, float volume, float initial_fraction)
        {
            var go = new GameObject("music");
            var asrc = go.AddComponent<AudioSource>();
            asrc.clip = clip;
            asrc.loop = true;
            asrc.priority = 0;
            DontDestroyOnLoad(go);

            global_audio_source = asrc;
            global_audio_volume = volume * initial_fraction;
            global_audio_volume_max = volume;
            ChangedMusicVolume();
            global_audio_source.Play();
        }

        void CheckBackgroundMusic()
        {
            if (global_audio_source == null)
            {
                MakeGlobalAudioSource(backgroundMusic, backgroundMusicVolume, 1f);
            }
            else if (global_audio_source.clip != backgroundMusic)
            {
                StartCoroutine(FadeOutMusic());
            }
            else
            {
                StartCoroutine(FadeInMusic());
            }
        }

        IEnumerator FadeOutMusic()
        {
            /* first, fade out the global audio source, over FADEOUT seconds.  Note that this
             * coroutine is destroyed if there is a scene change, but likely the new scene
             * will also run FadeOutMusic().  As an exception, if we switch to a scene with
             * the same music as the old clip, then we'll go to FadeInMusic() immediately. */
            const float FADEOUT = 2.2f;
            if (global_audio_volume > 0)
            {
                while (true)
                {
                    float vol = global_audio_volume;
                    vol -= global_audio_volume_max * Time.unscaledDeltaTime / FADEOUT;
                    if (vol <= 0f)
                        break;
                    global_audio_volume = vol;
                    ChangedMusicVolume();
                    yield return null;
                }
                global_audio_volume = 0;
                ChangedMusicVolume();

                /* wait for SILENT seconds */
                const float SILENT = 0.6f;
                yield return new WaitForSecondsRealtime(SILENT);
            }

            /* load the new music, playing but at volume 0 */
            Destroy(global_audio_source.gameObject);
            MakeGlobalAudioSource(backgroundMusic, backgroundMusicVolume, 0f);

            /* switch to FadeInMusic */
            StartCoroutine(FadeInMusic());
        }

        IEnumerator FadeInMusic()
        {
            /* fade in the global audio source, over FADEIN seconds.  Note that this
             * coroutine is destroyed if there is a scene change, but if the same clip
             * is found in the new scene, then the same FadeInMusic() will be called. */
            const float FADEIN = 0.9f;
            while (true)
            {
                if (music_volume_forced_reduced)
                    yield break;
                float vol = global_audio_volume;
                vol += global_audio_volume_max * Time.unscaledDeltaTime / FADEIN;
                if (vol >= global_audio_volume_max)
                    break;
                global_audio_volume = vol;
                ChangedMusicVolume();
                yield return null;
            }
            global_audio_volume = global_audio_volume_max;
            ChangedMusicVolume();
        }

        public static void ReduceMusicVolume(float fraction)
        {
            float vol = global_audio_volume_max * fraction;
            global_audio_volume = vol;
            ChangedMusicVolume();
            music_volume_forced_reduced = fraction < 1f;
        }

        public static void RestoreMusicVolume()
        {
            music_volume_forced_reduced = false;
            scene_loader.StartCoroutine(scene_loader.FadeInMusic());
        }

        public static void ChangedMusicVolume()
        {
            global_audio_source.volume = global_audio_volume * Level.GetLocalFile().music_volume;
        }

        struct SFX { internal List<AudioSource> asrcs; internal AudioSource org; }
        static Dictionary<string, SFX> _sfx;
        static Transform _sfx_parent;

        public const int MAX_SFX = 12;

        public static void Play2D(string name)
        {
            Play3D(name, Vector3.zero, 0f);
        }

        public static void Play3D(string name, Vector3Int position, float spatial_blend = 1f)
        {
            throw new Exception("Don't call Play3D with a Vector3Int!");
        }

        public static void Play3D(string name, Vector3 position, float spatial_blend = 1f, float delay = 0f)
        {
            if (_sfx == null)
            {
                _sfx_parent = new GameObject("sounds").transform;
                DontDestroyOnLoad(_sfx_parent.gameObject);

                _sfx = new Dictionary<string, SFX>();
                foreach (var asrc1 in game_prefabs.soundEffectsPrefab.GetComponentsInChildren<AudioSource>())
                {
                    _sfx.Add(asrc1.gameObject.name, new SFX
                    {
                        asrcs = new List<AudioSource>(),
                        org = asrc1,
                    });
                    Debug.Assert(!asrc1.playOnAwake);
                }
            }

            if (!_sfx.TryGetValue(name, out var sfx))
            {
                Debug.LogError("no sound with this name in the Sound Effects prefab: " + name);
                return;
            }

            AudioSource asrc = null;
            for (int i = 0; i < sfx.asrcs.Count; i++)
                if (!sfx.asrcs[i].isPlaying)
                {
                    asrc = sfx.asrcs[i];
                    break;
                }
            if (asrc == null)
            {
                /* stop if there are already MAX_SFX copies of this sound playing */
                if (sfx.asrcs.Count == MAX_SFX)
                    return;
                asrc = Instantiate(sfx.org, _sfx_parent);
                sfx.asrcs.Add(asrc);
            }

            asrc.transform.position = position;
            asrc.spatialBlend = spatial_blend;
            asrc.volume = sfx.org.volume * Level.GetLocalFile().effects_volume;
            asrc.PlayDelayed(delay);
        }


        /********** Coroutines and single-shot events with no obvious gameobject **********/

        class CustomCoroutines : MonoBehaviour { }

        static MonoBehaviour custom_coroutines;

        public static void StartCustomCoroutine(IEnumerator coroutine)
        {
            /* like StartCoroutine(), but can be used statically */
            if (custom_coroutines == null)
            {
                var go = new GameObject("coroutines");
                custom_coroutines = go.AddComponent<CustomCoroutines>();
                DontDestroyOnLoad(go);
            }
            custom_coroutines.StartCoroutine(coroutine);
        }

        static Action oneshot_event;
        static readonly object oneshot_lock = new object();

        public static void OneShotEvent(Action ev)
        {
            /* Arrange for ev() to be called at the next Update().  Can be called from any thread. */
            lock (oneshot_lock)
                oneshot_event += ev;
        }

        void Update()
        {
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             rb3.transform.Translate(0, 0, Time.deltaTime * 2);
            //             
            //             Action ev;
            //             lock (oneshot_lock)
            //             {
            //                 ev = oneshot_event;
            //                 if (ev == null)
            //                     return;
            //                 oneshot_event = null;
            //             }
            //             ev();
            //         }

            // FIXED VERSION:
