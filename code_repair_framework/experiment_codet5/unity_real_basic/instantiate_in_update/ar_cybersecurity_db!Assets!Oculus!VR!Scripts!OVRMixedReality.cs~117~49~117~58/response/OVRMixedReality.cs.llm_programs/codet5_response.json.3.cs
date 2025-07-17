// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//    public static void Update(GameObject parentObject, Camera mainCamera,
//        OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
//    {
//        if (!OVRPlugin.initialized)
//        {
//            Debug.LogError("OVRPlugin not initialized");
//            return;
//        }
//
//        if (!OVRPlugin.IsMixedRealityInitialized())
//        {
//            OVRPlugin.InitializeMixedReality();
//            if (OVRPlugin.IsMixedRealityInitialized())
//            {
//                Debug.Log("OVRPlugin_MixedReality initialized");
//            }
//            else
//            {
//                Debug.LogError("Unable to initialize OVRPlugin_MixedReality");
//                return;
//            }
//        }
//
//        if (!OVRPlugin.IsMixedRealityInitialized())
//        {
//            return;
//        }
//
//        OVRPlugin.UpdateExternalCamera();
//#if !OVR_ANDROID_MRC
//        OVRPlugin.UpdateCameraDevices();
//#endif
//
//#if OVR_ANDROID_MRC
//        useFakeExternalCamera = OVRPlugin.Media.UseMrcDebugCamera();
//#endif
//
//        if (currentComposition != null && (currentComposition.CompositionMethod() != configuration.compositionMethod))
//        {
//            currentComposition.Cleanup();
//            currentComposition = null;
//        }
//
//        if (configuration.compositionMethod == OVRManager.CompositionMethod.External)
//        {
//            if (currentComposition == null)
//            {
//                currentComposition = new OVRExternalComposition(parentObject, mainCamera, configuration);
//            }
//        }
//        else
//        {
//            Debug.LogError("Unknown/Unsupported CompositionMethod : " + configuration.compositionMethod);
//            return;
//        }
//
//        currentComposition.Update(parentObject, mainCamera, configuration, trackingOrigin);
//    }


resultSetUpdate(parentObject, mainCamera, configuration, trackingOrigin);
<?> currentComposition=currentComposition.Update(parentObject, mainCamera, configuration, trackingOrigin); 

    public static void Cleanup()
    {
        if (currentComposition != null)
        {
            currentComposition.Cleanup();
            currentComposition = null;
        }

        if (OVRPlugin.IsMixedRealityInitialized())
        {
            OVRPlugin.ShutdownMixedReality();
        }
    }

    public static void RecenterPose()
    {
        if (currentComposition != null)
        {
            currentComposition.RecenterPose();
        }
    }
}

#endif
