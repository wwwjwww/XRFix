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


//using System;
//using UnityEngine;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Text.RegularExpressions;
//using UnityEngine.XR.Management;

//public static class OVRMixedReality
//{
//    public static void Update(GameObject parentObject, Camera mainCamera,
//        OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
//    {
//        if (!XRGeneralSettings.Instance.ManagerActive &&!XRDevice.isPresent)
//        {
//            return;
//        }

//        if (!OVRPlugin.initialized)
//        {
//            Debug.LogError("OVRPlugin not initialized");
//            return;
//        }

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

//        if (!OVRPlugin.IsMixedRealityInitialized())
//        {
//            return;
//        }

//        OVRPlugin.UpdateExternalCamera();
//#if!OVR_ANDROID_MRC
//        OVRPlugin.UpdateCameraDevices();
//#endif

//        if (currentComposition!= null && (currentComposition.CompositionMethod()!= configuration.compositionMethod))
//        {
//

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
