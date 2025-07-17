


using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    
    
    
    
    
    [HelpURL("https:
    public partial class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
        
        
        
        
        const float SceneActivationLoadProgress = 0.9f;

        
        
        
        private enum SceneType
        {
            Manager = 0,
            Content = 1,
            Lighting = 2,
        }

        public MixedRealitySceneSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySceneSystemProfile profile) : base(registrar, profile)
        {
            this.profile = profile;
        }

        private MixedRealitySceneSystemProfile profile;

        
        private bool managerSceneOpInProgress;
        private float managerSceneOpProgress;

        
        private SceneContentTracker contentTracker;
        
        private SceneLightingExecutor lightingExecutor;

        
        public override string Name { get; protected set; } = "Mixed Reality Scene System";

        #region Actions

        
        public Action<IEnumerable<string>> OnWillLoadContent { get; set; }

        
        public Action<IEnumerable<string>> OnContentLoaded { get; set; }

        
        public Action<IEnumerable<string>> OnWillUnloadContent { get; set; }

        
        public Action<IEnumerable<string>> OnContentUnloaded { get; set; }

        
        public Action<string> OnWillLoadLighting { get; set; }

        
        public Action<string> OnLightingLoaded { get; set; }

        
        public Action<string> OnWillUnloadLighting { get; set; }

        
        public Action<string> OnLightingUnloaded { get; set; }

        
        public Action<string> OnWillLoadScene { get; set; }

        
        public Action<string> OnSceneLoaded { get; set; }

        
        public Action<string> OnWillUnloadScene { get; set; }

        
        public Action<string> OnSceneUnloaded { get; set; }

        #endregion

        #region Properties

        
        public bool SceneOperationInProgress { get; private set; } = false;

        
        public float SceneOperationProgress { get; private set; } = 0;

        
        public bool LightingOperationInProgress { get; private set; } = false;

        
        public float LightingOperationProgress { get; private set; } = 0;

        
        public string ActiveLightingScene { get; private set; } = string.Empty;

        
        public bool WaitingToProceed { get; private set; } = false;

        
        public bool PrevContentExists => contentTracker.PrevContentExists;

        
        public bool NextContentExists => contentTracker.NextContentExists;

        
        public string[] ContentSceneNames => contentTracker.ContentSceneNames;

        
        public uint SourceId { get; } = 0;

        
        public string SourceName { get; } = "Mixed Reality Scene System";

        #endregion

        #region Service Methods

        
        public override void Initialize()
        {
            
            contentTracker = new SceneContentTracker(profile);
            lightingExecutor = new SceneLightingExecutor();

#if UNITY_EDITOR
            EditorOnInitialize();
#endif

            if (!Application.isPlaying)
            {
                return;
            }

            if (profile.UseManagerScene)
            {
                SetManagerScene(profile.ManagerScene.Name);
            }

            if (profile.UseLightingScene)
            {   
                SetLightingScene(profile.DefaultLightingScene.Name, LightingSceneTransitionType.None);
            }
        }

        
        public override void Enable()
        {
#if UNITY_EDITOR
            EditorOnDisable();
#endif
        }

        
        public override void Disable()
        {
#if UNITY_EDITOR
            EditorOnDisable();
#endif
        }

        
        public override void Destroy()
        {
#if UNITY_EDITOR
            EditorOnDestroy();
#endif
        }

        
        public override void Update()
        {
            
            if (profile.UseLightingScene)
            {
                lightingExecutor.UpdateTransition(Time.unscaledDeltaTime);
            }
        }

        #endregion

        #region Scene Operations

        
        public async Task LoadNextContent(bool wrap = false, LoadSceneMode mode = LoadSceneMode.Single, SceneActivationToken activationToken = null)
        {
            string nextContent;
            if (contentTracker.GetNextContent(wrap, out nextContent))
            {
                await LoadContent(new string[] { nextContent }, mode, activationToken);
            }
            else
            {
                Debug.LogWarning("Attempted to load next content when no next content exists. Taking no action.");
            }
        }

        
        public async Task LoadPrevContent(bool wrap = false, LoadSceneMode mode = LoadSceneMode.Single, SceneActivationToken activationToken = null)
        {
            string prevContent;
            if (contentTracker.GetPrevContent(wrap, out prevContent))
            {
                await LoadContent(new string[] { prevContent }, mode, activationToken);
            }
            else
            {
                Debug.LogWarning("Attempted to load prev content when no next content exists. Taking no action.");
            }
        }

        
        public async Task LoadContent(string sceneToLoad, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            await LoadContent(new string[] { sceneToLoad }, mode, activationToken);
        }

        
        public async Task UnloadContent(string sceneToUnload)
        {
            await UnloadContent(new string[] { sceneToUnload });
        }

        
        public async Task LoadContentByTag(string tag, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            await LoadContent(profile.GetContentSceneNamesByTag(tag), mode, activationToken);
        }

        
        public async Task UnloadContentByTag(string tag)
        {
            await UnloadScenesInternal(profile.GetContentSceneNamesByTag(tag), SceneType.Content);
        }

        
        public async Task LoadContent(IEnumerable<string> scenesToLoad, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            if (!CanSceneOpProceed(SceneType.Content))
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            IEnumerable<string> loadedContentScenes;
            if (mode == LoadSceneMode.Single && GetLoadedContentScenes(out loadedContentScenes))
            {
                await UnloadScenesInternal(loadedContentScenes, SceneType.Content, 0, 0.5f, true);
                await LoadScenesInternal(scenesToLoad, SceneType.Content, activationToken, 0.5f, 1f, false);
            }
            else
            {
                await LoadScenesInternal(scenesToLoad, SceneType.Content, activationToken);
            }

            await LoadScenesInternal(scenesToLoad, SceneType.Content, activationToken);
        }

        
        public async Task UnloadContent(IEnumerable<string> scenesToUnload)
        {
            if (!CanSceneOpProceed(SceneType.Content))
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            await UnloadScenesInternal(scenesToUnload, SceneType.Content);
        }

        
        public bool IsContentLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        
        public async void SetLightingScene(string newLightingSceneName, LightingSceneTransitionType transitionType = LightingSceneTransitionType.None, float transitionDuration = 1f)
        {
            if (ActiveLightingScene == newLightingSceneName)
            {   
                return;
            }

            if (!CanSceneOpProceed(SceneType.Lighting))
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            SceneInfo lightingScene;
            RuntimeLightingSettings lightingSettings = default(RuntimeLightingSettings);
            RuntimeRenderSettings renderSettings = default(RuntimeRenderSettings);
            RuntimeSunlightSettings sunSettings = default(RuntimeSunlightSettings);
            if (!string.IsNullOrEmpty(newLightingSceneName) && !profile.GetLightingSceneSettings(
                newLightingSceneName,
                out lightingScene, 
                out lightingSettings, 
                out renderSettings,
                out sunSettings))
            {   
                Debug.LogWarning("Couldn't find lighting scene " + newLightingSceneName + " in profile - taking no action.");
                return;
            }

            ActiveLightingScene = newLightingSceneName;

            if (!Application.isPlaying)
            {   
                return;
            }

            
            lightingExecutor.StartTransition(lightingSettings, renderSettings, sunSettings, transitionType, transitionDuration);

            List<string> lightingSceneNames = new List<string>();
            
            foreach (SceneInfo lso in profile.LightingScenes)
            {
                if (lso.Name != newLightingSceneName)
                {
                    lightingSceneNames.Add(lso.Name);
                }
            }

            
            await LoadScenesInternal(new string[] { newLightingSceneName }, SceneType.Lighting, null, 0f, 0.5f, true);

            
            await UnloadScenesInternal(lightingSceneNames, SceneType.Lighting, 0.5f, 1f, false);
        }

        
        
        
        private async void SetManagerScene(string managerSceneName)
        {
            Scene scene = SceneManager.GetSceneByName(managerSceneName);
            if (scene.IsValid() && !scene.isLoaded)
            {   
                return;
            }

            await LoadScenesInternal(new string[] { managerSceneName }, SceneType.Manager);
        }

        
        
        
        private async Task LoadScenesInternal(
            IEnumerable<string> scenesToLoad,
            SceneType sceneType,
            SceneActivationToken activationToken = null,
            float progressOffset = 0,
            float progressTarget = 1,
            bool sceneOpInProgressWhenFinished = false)
        {
            
            activationToken?.SetReadyToProceed(false);

            SetSceneOpProgress(true, progressOffset, sceneType);

            
            List<string> validNames = new List<string>();
            List<int> validIndexes = new List<int>();

            foreach (string sceneName in scenesToLoad)
            {
                
                Scene scene;
                int sceneIndex;
                if (!RuntimeSceneUtils.FindScene(sceneName, out scene, out sceneIndex))
                {
                    Debug.LogError("Can't load invalid scene " + sceneName);
                }
                else
                {
                    validIndexes.Add(sceneIndex);
                    validNames.Add(sceneName);
                }
            }

            int totalSceneOps = validIndexes.Count;
            if (totalSceneOps < 1)
            {
                Debug.LogWarning("No valid scenes found to load.");
                SetSceneOpProgress(sceneOpInProgressWhenFinished, progressTarget, sceneType);
                return;
            }

            
            InvokeWillLoadActions(validNames, sceneType);

            
            if (validIndexes.Count > 0)
            {
                List<AsyncOperation> loadSceneOps = new List<AsyncOperation>();
                foreach (int sceneIndex in validIndexes)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    if (scene.isLoaded)
                        continue;

                    AsyncOperation sceneOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                    
                    sceneOp.allowSceneActivation = (activationToken != null) ? activationToken.AllowSceneActivation : true;
                    loadSceneOps.Add(sceneOp);
                }

                
                bool completedAllSceneOps = false;

                while (!completedAllSceneOps)
                {
                    if (!Application.isPlaying)
                    {   
                        return;
                    }

                    completedAllSceneOps = true;
                    bool readyToProceed = false;
                    bool allowSceneActivation = (activationToken != null) ? activationToken.AllowSceneActivation : true;

                    
                    float sceneOpProgress = 0;
                    for (int i = 0; i < loadSceneOps.Count; i++)
                    {
                        
                        
                        loadSceneOps[i].allowSceneActivation = allowSceneActivation;

                        if (loadSceneOps[i].isDone)
                        {   
                            
                            
                            sceneOpProgress += 1;
                        }
                        else
                        {
                            readyToProceed |= loadSceneOps[i].progress >= SceneActivationLoadProgress;
                            sceneOpProgress += loadSceneOps[i].progress;
                            completedAllSceneOps = false;
                        }
                    }

                    
                    activationToken?.SetReadyToProceed(readyToProceed);

                    sceneOpProgress = Mathf.Clamp01(SceneOperationProgress / totalSceneOps);
                    
                    SetSceneOpProgress(true, Mathf.Lerp(progressOffset, progressTarget, sceneOpProgress), sceneType);

                    await Task.Yield();
                }
            }

            
            bool scenesLoadedAndActivated = false;
            while (!scenesLoadedAndActivated)
            {
                if (!Application.isPlaying)
                {   
                    return;
                }

                scenesLoadedAndActivated = true;
                foreach (int sceneIndex in validIndexes)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    // BUG: Potentially dangerous use of non-short-circuit logic
                    // MESSAGE: The & and | operators do not use short-circuit evaluation and can be dangerous when applied to boolean operands. In particular, their use can result in errors if the left-hand operand checks for cases in which it is not safe to evaluate the right-hand one.
                    //                     scenesLoadedAndActivated &= (scene.IsValid() & scene.isLoaded);

                    // FIXED CODE:
