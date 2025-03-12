    void Awake()
     {
        objectPool = new Queue<GameObject>();
        // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
        for (int i = 0; i < 10; i++)
        {
            var pooledObject = Object.Instantiate(mainCamera.gameObject);
            pooledObject.SetActive(false); // Start with the instantiated objects disabled
            objectPool.Enqueue(pooledObject);
        }
     }

    private void RefreshCameraObjects(GameObject parentObject, Camera mainCamera,
        OVRMixedRealityCaptureConfiguration configuration)
    {
        if (mainCamera.gameObject != previousMainCameraObject)
        {
            Debug.LogFormat("[OVRExternalComposition] Camera refreshed. Rebind camera to {0}",
                mainCamera.gameObject.name);

            OVRCompositionUtil.SafeDestroy(ref backgroundCameraGameObject);
            backgroundCamera = null;
            OVRCompositionUtil.SafeDestroy(ref foregroundCameraGameObject);
            foregroundCamera = null;

            RefreshCameraRig(parentObject, mainCamera);

            Debug.Assert(backgroundCameraGameObject == null);
            if (configuration.instantiateMixedRealityCameraGameObject != null)
            {
                backgroundCameraGameObject =
                    configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject,
                        OVRManager.MrcCameraType.Background);
            }
            else
            {
                var backgroundCameraGameObject = objectPool.Dequeue();
                backgroundCameraGameObject.SetActive(true);
            }

            backgroundCameraGameObject.name = "OculusMRC_BackgroundCamera";
            backgroundCameraGameObject.transform.parent =
                cameraInTrackingSpace ? cameraRig.trackingSpace : parentObject.transform;
            if (backgroundCameraGameObject.GetComponent<AudioListener>())
            {
               Object.Destroy(backgroundCameraGameObject.GetComponent<AudioListener>());
            }

            if (backgroundCameraGameObject.GetComponent<OVRManager>())
            {
                Object.Destroy(backgroundCameraGameObject.GetComponent<OVRManager>());
            }

            backgroundCamera = backgroundCameraGameObject.GetComponent<Camera>();
            backgroundCamera.tag = "Untagged";
#if USING_MRC_COMPATIBLE_URP_VERSION
            var backgroundCamData = backgroundCamera.GetUniversalAdditionalCameraData();
            if (backgroundCamData != null)
            {
                backgroundCamData.allowXRRendering = false;
            }
#elif USING_URP
            Debug.LogError("Using URP with MRC is only supported with URP version 10.0.0 or higher. Consider using Unity 2020 or higher.");
#else
            backgroundCamera.stereoTargetEye = StereoTargetEyeMask.None;
#endif
            backgroundCamera.depth = 99990.0f;
            backgroundCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
            backgroundCamera.cullingMask = (backgroundCamera.cullingMask & ~configuration.extraHiddenLayers) |
                                           configuration.extraVisibleLayers;
#if OVR_ANDROID_MRC
            backgroundCamera.targetTexture = mrcRenderTextureArray[0];
            if (!renderCombinedFrame)
            {
                backgroundCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            }
#endif

            Debug.Assert(foregroundCameraGameObject == null);
            if (configuration.instantiateMixedRealityCameraGameObject != null)
            {
                foregroundCameraGameObject =
                    configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject,
                        OVRManager.MrcCameraType.Foreground);
            }
            else
            {
                foregroundCameraGameObject = Object.Instantiate(mainCamera.gameObject);
            }

            foregroundCameraGameObject.name = "OculusMRC_ForgroundCamera";
            foregroundCameraGameObject.transform.parent =
                cameraInTrackingSpace ? cameraRig.trackingSpace : parentObject.transform;
            if (foregroundCameraGameObject.GetComponent<AudioListener>())
            {
                Object.Destroy(foregroundCameraGameObject.GetComponent<AudioListener>());
            }//
            if (foregroundCameraGameObject.GetComponent<OVRManager>())
            {
                Object.Destroy(foregroundCameraGameObject.GetComponent<OVRManager>());
            }

            foregroundCamera = foregroundCameraGameObject.GetComponent<Camera>();
            foregroundCamera.tag = "Untagged";
#if USING_MRC_COMPATIBLE_URP_VERSION
            var foregroundCamData = foregroundCamera.GetUniversalAdditionalCameraData();
            if (foregroundCamData != null)
            {
                foregroundCamData.allowXRRendering = false;
            }
#elif USING_URP
            Debug.LogError("Using URP with MRC is only supported with URP version 10.0.0 or higher. Consider using Unity 2020 or higher.");
#else
            foregroundCamera.stereoTargetEye = StereoTargetEyeMask.None;
#endif
            foregroundCamera.depth =
                backgroundCamera.depth + 1.0f; // enforce the forground be rendered after the background
            foregroundCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
            foregroundCamera.clearFlags = CameraClearFlags.Color;
#if OVR_ANDROID_MRC
            foregroundCamera.backgroundColor = configuration.externalCompositionBackdropColorQuest;
#else
            foregroundCamera.backgroundColor = configuration.externalCompositionBackdropColorRift;
#endif
            foregroundCamera.cullingMask = (foregroundCamera.cullingMask & ~configuration.extraHiddenLayers) |
                                           configuration.extraVisibleLayers;

#if OVR_ANDROID_MRC
            if (renderCombinedFrame)
            {
                foregroundCamera.targetTexture = mrcRenderTextureArray[0];
            }
            else
            {
                foregroundCamera.targetTexture = mrcForegroundRenderTextureArray[0];
                foregroundCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            }
#endif








            previousMainCameraObject = mainCamera.gameObject;
        }
    }