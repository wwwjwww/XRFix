     void Awake()
     {
        objectPool = new Queue<GameObject>();
        // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
        for (int i = 0; i < 10; i++)
        {
            var pooledObject = Instantiate(mainCamera);
            pooledObject.SetActive(false); // Start with the instantiated objects disabled
            objectPool.Enqueue(pooledObject);
        }
     }

    public static void Update(GameObject parentObject, Camera mainCamera,
        OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
    {
        if (!OVRPlugin.initialized)
        {
            Debug.LogError("OVRPlugin not initialized");
            return;
        }

        if (!OVRPlugin.IsMixedRealityInitialized())
        {
            OVRPlugin.InitializeMixedReality();
            if (OVRPlugin.IsMixedRealityInitialized())
            {
                Debug.Log("OVRPlugin_MixedReality initialized");
            }
            else
            {
                Debug.LogError("Unable to initialize OVRPlugin_MixedReality");
                return;
            }
        }

        if (!OVRPlugin.IsMixedRealityInitialized())
        {
            return;
        }

        OVRPlugin.UpdateExternalCamera();
#if !OVR_ANDROID_MRC
        OVRPlugin.UpdateCameraDevices();
#endif

#if OVR_ANDROID_MRC
        useFakeExternalCamera = OVRPlugin.Media.UseMrcDebugCamera();
#endif

        if (currentComposition != null && (currentComposition.CompositionMethod() != configuration.compositionMethod))
        {
            currentComposition.Cleanup();
            currentComposition = null;
        }

        if (configuration.compositionMethod == OVRManager.CompositionMethod.External)
        {
            if (currentComposition == null)
            {
                currentComposition = new OVRExternalComposition(parentObject, mainCamera, configuration);
            }
        }
        else
        {
            Debug.LogError("Unknown/Unsupported CompositionMethod : " + configuration.compositionMethod);
            return;
        }

        var currentComposition = objectPool.Dequeue();
        currentComposition.SetActive(true);
    }