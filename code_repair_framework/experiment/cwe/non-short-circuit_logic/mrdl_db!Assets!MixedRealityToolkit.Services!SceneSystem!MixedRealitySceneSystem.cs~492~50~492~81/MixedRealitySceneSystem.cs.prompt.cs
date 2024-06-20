// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    /// <summary>
    /// The default implementation of the <see cref="Microsoft.MixedReality.Toolkit.SceneSystem.IMixedRealitySceneSystem"/>
    /// Because so much of this service's functionality is editor-only, it has been split into a partial class.
    /// This part handles the runtime parts of the service.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SceneSystem/SceneSystemGettingStarted.html")]
    public partial class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
        /// <summary>
        /// Async load operation progress amount indicating that we're ready to activate a scene.
        /// https://docs.unity3d.com/ScriptReference/AsyncOperation-progress.html
        /// </summary>
        const float SceneActivationLoadProgress = 0.9f;

        /// <summary>
        /// Used by internal load methods to decide which actions to invoke.
        /// </summary>
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

        // Internal scene operation info
        private bool managerSceneOpInProgress;
        private float managerSceneOpProgress;

        // Content tracker instance
        private SceneContentTracker contentTracker;
        // Lighting executor instance
        private SceneLightingExecutor lightingExecutor;

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Scene System";

        #region Actions

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnWillLoadContent { get; set; }

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnContentLoaded { get; set; }

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnWillUnloadContent { get; set; }

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnContentUnloaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillLoadLighting { get; set; }

        /// <inheritdoc />
        public Action<string> OnLightingLoaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillUnloadLighting { get; set; }

        /// <inheritdoc />
        public Action<string> OnLightingUnloaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillLoadScene { get; set; }

        /// <inheritdoc />
        public Action<string> OnSceneLoaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillUnloadScene { get; set; }

        /// <inheritdoc />
        public Action<string> OnSceneUnloaded { get; set; }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool SceneOperationInProgress { get; private set; } = false;

        /// <inheritdoc />
        public float SceneOperationProgress { get; private set; } = 0;

        /// <inheritdoc />
        public bool LightingOperationInProgress { get; private set; } = false;

        /// <inheritdoc />
        public float LightingOperationProgress { get; private set; } = 0;

        /// <inheritdoc />
        public string ActiveLightingScene { get; private set; } = string.Empty;

        /// <inheritdoc />
        public bool WaitingToProceed { get; private set; } = false;

        /// <inheritdoc />
        public bool PrevContentExists => contentTracker.PrevContentExists;

        /// <inheritdoc />
        public bool NextContentExists => contentTracker.NextContentExists;

        /// <inheritdoc />
        public string[] ContentSceneNames => contentTracker.ContentSceneNames;

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Scene System";

        #endregion

        #region Service Methods

        /// <inheritdoc />
        public override void Initialize()
        {
            // Create a new instance of our content tracker
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
            {   // Set our lighting scene immediately, with no transition
                SetLightingScene(profile.DefaultLightingScene.Name, LightingSceneTransitionType.None);
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
#if UNITY_EDITOR
            EditorOnDisable();
#endif
        }

        /// <inheritdoc />
        public override void Disable()
        {
#if UNITY_EDITOR
            EditorOnDisable();
#endif
        }

        /// <inheritdoc />
        public override void Destroy()
        {
#if UNITY_EDITOR
            EditorOnDestroy();
#endif
        }

        /// <inheritdoc />
        public override void Update()
        {
            // Ensure the lighting scene is active, if we're using one.
            if (profile.UseLightingScene)
            {
                lightingExecutor.UpdateTransition(Time.unscaledDeltaTime);
            }
        }

        #endregion

        #region Scene Operations

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task LoadContent(string sceneToLoad, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            await LoadContent(new string[] { sceneToLoad }, mode, activationToken);
        }

        /// <inheritdoc />
        public async Task UnloadContent(string sceneToUnload)
        {
            await UnloadContent(new string[] { sceneToUnload });
        }

        /// <inheritdoc />
        public async Task LoadContentByTag(string tag, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            await LoadContent(profile.GetContentSceneNamesByTag(tag), mode, activationToken);
        }

        /// <inheritdoc />
        public async Task UnloadContentByTag(string tag)
        {
            await UnloadScenesInternal(profile.GetContentSceneNamesByTag(tag), SceneType.Content);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task UnloadContent(IEnumerable<string> scenesToUnload)
        {
            if (!CanSceneOpProceed(SceneType.Content))
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            await UnloadScenesInternal(scenesToUnload, SceneType.Content);
        }

        /// <inheritdoc />
        public bool IsContentLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        /// <inheritdoc />
        public async void SetLightingScene(string newLightingSceneName, LightingSceneTransitionType transitionType = LightingSceneTransitionType.None, float transitionDuration = 1f)
        {
            if (ActiveLightingScene == newLightingSceneName)
            {   // Nothing to do here
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
            {   // Make sure we don't try to load a non-existent scene
                Debug.LogWarning("Couldn't find lighting scene " + newLightingSceneName + " in profile - taking no action.");
                return;
            }

            ActiveLightingScene = newLightingSceneName;

            if (!Application.isPlaying)
            {   // Everything else is runtime-only
                return;
            }

            // Start the lighting executor transition - don't bother waiting for load / unload, it can start right away
            lightingExecutor.StartTransition(lightingSettings, renderSettings, sunSettings, transitionType, transitionDuration);

            List<string> lightingSceneNames = new List<string>();
            // Create a list of lighting scenes to unload
            foreach (SceneInfo lso in profile.LightingScenes)
            {
                if (lso.Name != newLightingSceneName)
                {
                    lightingSceneNames.Add(lso.Name);
                }
            }

            // Load the new lighting scene immediately
            await LoadScenesInternal(new string[] { newLightingSceneName }, SceneType.Lighting, null, 0f, 0.5f, true);

            // Unload the other lighting scenes
            await UnloadScenesInternal(lightingSceneNames, SceneType.Lighting, 0.5f, 1f, false);
        }

        /// <summary>
        /// Loads the manager scene.
        /// </summary>
        private async void SetManagerScene(string managerSceneName)
        {
            Scene scene = SceneManager.GetSceneByName(managerSceneName);
            if (scene.IsValid() && !scene.isLoaded)
            {   // If the manager scene is already loaded, don't proceed.
                return;
            }

            await LoadScenesInternal(new string[] { managerSceneName }, SceneType.Manager);
        }

        /// <summary>
        /// Internal method to handle scene loads
        /// </summary>
        private async Task LoadScenesInternal(
            IEnumerable<string> scenesToLoad,
            SceneType sceneType,
            SceneActivationToken activationToken = null,
            float progressOffset = 0,
            float progressTarget = 1,
            bool sceneOpInProgressWhenFinished = false)
        {
                    // BUG: Potentially dangerous use of non-short-circuit logic
                    // MESSAGE: The & and | operators do not use short-circuit evaluation and can be dangerous when applied to boolean operands. In particular, their use can result in errors if the left-hand operand checks for cases in which it is not safe to evaluate the right-hand one.
                    //             // If we're using an activation token let it know that we're NOT ready to proceed
                    //             activationToken?.SetReadyToProceed(false);
                    // 
                    //             SetSceneOpProgress(true, progressOffset, sceneType);
                    // 
                    //             // Validate our scenes
                    //             List<string> validNames = new List<string>();
                    //             List<int> validIndexes = new List<int>();
                    // 
                    //             foreach (string sceneName in scenesToLoad)
                    //             {
                    //                 // See if scene exists
                    //                 Scene scene;
                    //                 int sceneIndex;
                    //                 if (!RuntimeSceneUtils.FindScene(sceneName, out scene, out sceneIndex))
                    //                 {
                    //                     Debug.LogError("Can't load invalid scene " + sceneName);
                    //                 }
                    //                 else
                    //                 {
                    //                     validIndexes.Add(sceneIndex);
                    //                     validNames.Add(sceneName);
                    //                 }
                    //             }
                    // 
                    //             int totalSceneOps = validIndexes.Count;
                    //             if (totalSceneOps < 1)
                    //             {
                    //                 Debug.LogWarning("No valid scenes found to load.");
                    //                 SetSceneOpProgress(sceneOpInProgressWhenFinished, progressTarget, sceneType);
                    //                 return;
                    //             }
                    // 
                    //             // We're about to load scenes - let everyone know
                    //             InvokeWillLoadActions(validNames, sceneType);
                    // 
                    //             // Load our scenes
                    //             if (validIndexes.Count > 0)
                    //             {
                    //                 List<AsyncOperation> loadSceneOps = new List<AsyncOperation>();
                    //                 foreach (int sceneIndex in validIndexes)
                    //                 {
                    //                     Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    //                     if (scene.isLoaded)
                    //                         continue;
                    // 
                    //                     AsyncOperation sceneOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                    //                     // Set this to true unless we have an activation token
                    //                     sceneOp.allowSceneActivation = (activationToken != null) ? activationToken.AllowSceneActivation : true;
                    //                     loadSceneOps.Add(sceneOp);
                    //                 }
                    // 
                    //                 // Now wait for all async operations to complete
                    //                 bool completedAllSceneOps = false;
                    // 
                    //                 while (!completedAllSceneOps)
                    //                 {
                    //                     if (!Application.isPlaying)
                    //                     {   // Break out of this loop if we've stopped playmode
                    //                         return;
                    //                     }
                    // 
                    //                     completedAllSceneOps = true;
                    //                     bool readyToProceed = false;
                    //                     bool allowSceneActivation = (activationToken != null) ? activationToken.AllowSceneActivation : true;
                    // 
                    //                     // Go through all the load scene ops and see if we're ready to be activated
                    //                     float sceneOpProgress = 0;
                    //                     for (int i = 0; i < loadSceneOps.Count; i++)
                    //                     {
                    //                         // Set allow scene activation
                    //                         // (This can be set to true by user before ReadyToProceed is set)
                    //                         loadSceneOps[i].allowSceneActivation = allowSceneActivation;
                    // 
                    //                         if (loadSceneOps[i].isDone)
                    //                         {   // Sometimes if a scene is small enough, progress will get reset to 0 before you even have a chance to check it
                    //                             // This is true EVEN IF you've set allowSceneActivation to false
                    //                             // So use isDone as a failsafe
                    //                             sceneOpProgress += 1;
                    //                         }
                    //                         else
                    //                         {
                    //                             readyToProceed |= loadSceneOps[i].progress >= SceneActivationLoadProgress;
                    //                             sceneOpProgress += loadSceneOps[i].progress;
                    //                             completedAllSceneOps = false;
                    //                         }
                    //                     }
                    // 
                    //                     // Let the activation know whether we're ready
                    //                     activationToken?.SetReadyToProceed(readyToProceed);
                    // 
                    //                     sceneOpProgress = Mathf.Clamp01(SceneOperationProgress / totalSceneOps);
                    //                     
                    //                     SetSceneOpProgress(true, Mathf.Lerp(progressOffset, progressTarget, sceneOpProgress), sceneType);
                    // 
                    //                     await Task.Yield();
                    //                 }
                    //             }
                    // 
                    //             // Wait for all scenes to be fully loaded before proceeding
                    //             bool scenesLoadedAndActivated = false;
                    //             while (!scenesLoadedAndActivated)
                    //             {
                    //                 if (!Application.isPlaying)
                    //                 {   // Break out of this loop if we've stopped playmode
                    //                     return;
                    //                 }
                    // 
                    //                 scenesLoadedAndActivated = true;
                    //                 foreach (int sceneIndex in validIndexes)
                    //                 {
                    //                     Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    //                     scenesLoadedAndActivated &= (scene.IsValid() & scene.isLoaded);
                    //                 }
                    //                 await Task.Yield();
                    //             }
                    // 
                    //             // Make sure our content tracker is refreshed
                    //             contentTracker.RefreshLoadedContent();
                    // 
                    //             // We're done!
                    //             SetSceneOpProgress(sceneOpInProgressWhenFinished, progressTarget, sceneType);
                    //             
                    //             InvokeLoadedActions(validNames, sceneType);
                    //         }

                    // FIXED VERSION:
