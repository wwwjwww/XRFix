/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     public static void Update(GameObject parentObject, Camera mainCamera,
*         OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
*     {
*         if (!OVRPlugin.initialized)
*         {
*             Debug.LogError("OVRPlugin not initialized");
*             return;
*         }
* 
*         if (!OVRPlugin.IsMixedRealityInitialized())
*         {
*             OVRPlugin.InitializeMixedReality();
*             if (OVRPlugin.IsMixedRealityInitialized())
*             {
*                 Debug.Log("OVRPlugin_MixedReality initialized");
*             }
*             else
*             {
*                 Debug.LogError("Unable to initialize OVRPlugin_MixedReality");
*                 return;
*             }
*         }
* 
*         if (!OVRPlugin.IsMixedRealityInitialized())
*         {
*             return;
*         }
* 
*         OVRPlugin.UpdateExternalCamera();
* #if !OVR_ANDROID_MRC
*         OVRPlugin.UpdateCameraDevices();
* #endif
* 
* #if OVR_ANDROID_MRC
*         useFakeExternalCamera = OVRPlugin.Media.UseMrcDebugCamera();
* #endif
* 
*         if (currentComposition != null && (currentComposition.CompositionMethod() != configuration.compositionMethod))
*         {
*             currentComposition.Cleanup();
*             currentComposition = null;
*         }
* 
*         if (configuration.compositionMethod == OVRManager.CompositionMethod.External)
*         {
*             if (currentComposition == null)
*             {
*                 currentComposition = new OVRExternalComposition(parentObject, mainCamera, configuration);
*             }
*         }
*         else
*         {
*             Debug.LogError("Unknown/Unsupported CompositionMethod : " + configuration.compositionMethod);
*             return;
*         }
* 
*         currentComposition.Update(parentObject, mainCamera, configuration, trackingOrigin);
*     }


using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using Unity.ObjectPool;

/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     public static void Update(GameObject parentObject, Camera mainCamera,
*         OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
*     {
*         if (!OVRPlugin.initialized)
*         {
*             Debug.LogError("OVRPlugin not initialized");
*             return;
*         }
* 
*         if (!OVRPlugin.IsMixedRealityInitialized())
*         {
*             OVRPlugin.InitializeMixedReality();
*             if (OVRPlugin.IsMixedRealityInitialized())
*             {
*                 Debug.Log("OVRPlugin_MixedReality initialized");
*             }
*             else
*             {
*                 Debug.LogError("Unable to initialize OVRPlugin_MixedReality");
*                 return;
*             }
*         }
* 
*         if (!OVRPlugin.IsMixedRealityInitialized())
*         {
*             return;
*         }
* 
*         OVRPlugin.UpdateExternalCamera();
* #if !OVR_ANDROID_MRC
*         OVRPlugin.UpdateCameraDevices();
* #endif
* 
* #if OVR_ANDROID_MRC
*         useFakeExternalCamera = OVRPlugin.Media.UseMrcDebugCamera();
* #endif
* 
*         if (currentComposition != null && (currentComposition.CompositionMethod() != configuration.compositionMethod))
*         {
*             currentComposition.Cleanup();
*             currentComposition = null;
*         }
* 
*         if (configuration.compositionMethod == OVRManager.CompositionMethod.External)
*         {
*             if (currentComposition == null)
*             {
if( ObjectPool.Get().length == 0)
{
*                 currentComposition = new OVRExternalComposition(parentObject, mainCamera, configuration);
}
else
{
*                 currentComposition = ObjectPool.Get().PopLast();
*                 currentComposition.Init(parentObject, mainCamera, configuration);
}
}
*         }
*         else
*         {
*             Debug.LogError("Unknown/Unsupported CompositionMethod : " + configuration.compositionMethod);
*             return;
*         }
* 
*         currentComposition.Update(parentObject, mainCamera, configuration, trackingOrigin);
*     }

* In OVRExternalComposition class, add object pool class to call the Cleanup() method in the destructor.
*/
using System; using UnityEngine; using System.Collections.Generic; using System.Threading; using UnityEngine.Rendering; using Object = UnityEngine.Object; using UnityEngine.Rendering.Universal;

// Here is the definition of the OVRExternalComposition class in /Assets/Oculus/VR/Scripts/Composition/OVRExternalComposition.cs
*    
*     private void RefreshCameraObjects(GameObject parentObject, Camera mainCamera,
*         OVRMixedRealityCaptureConfiguration configuration) { if (mainCamera.gameObject != previousMainCameraObject) { Debug.LogFormat("[OVRExternalComposition] Camera refreshed. Rebind camera to {0}",       mainCamera.gameObject.name);
* OVRCompositionUtil.SafeDestroy(ref backgroundCameraGameObject); backgroundCamera = null; } OVRCompositionUtil.SafeDestroy(ref foregroundCameraGameObject); foregroundCamera = null; RefreshCameraRig(parentObject, mainCamera);
* Debug.Assert(backgroundCameraGameObject == null); if (configuration.instantiateMixedRealityCameraGameObject != null) { backgroundCameraGameObject = configuration.instantiateMixedRealityCameraGameObject     (ma 
{ OVRObjectPool.Get().Add(this); } 

* In the destructor of OVRExternalComposition, call Cleanup() method.
*/

*     ~OVRExternalComposition()
*     {
*         Cleanup();
*     }
*     }

* Finally, remove "using UnityEngine.Object;", "using UnityEngine.Rendering.Universal;", and "OVRCompositionUtil.SafeDestroy(ref foregroundCameraGameObject);" from the code.

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
