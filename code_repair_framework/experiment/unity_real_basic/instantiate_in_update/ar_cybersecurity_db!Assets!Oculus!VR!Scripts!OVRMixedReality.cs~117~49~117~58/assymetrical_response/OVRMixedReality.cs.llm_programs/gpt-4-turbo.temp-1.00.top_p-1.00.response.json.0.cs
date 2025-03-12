/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#if UNITY_ANDROID && !UNITY_EDITOR
#define OVR_ANDROID_MRC
#endif

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_ANDROID




internal static class OVRMixedReality
{



    public static bool useFakeExternalCamera = false;

    public static Vector3 fakeCameraFloorLevelPosition = new Vector3(0.0f, 2.0f, -0.5f);
    public static Vector3 fakeCameraEyeLevelPosition = fakeCameraFloorLevelPosition - new Vector3(0.0f, 1.8f, 0.0f);

    public static Quaternion fakeCameraRotation = Quaternion.LookRotation(
        (new Vector3(0.0f, fakeCameraFloorLevelPosition.y, 0.0f) - fakeCameraFloorLevelPosition).normalized,
        Vector3.up);

    public static float fakeCameraFov = 60.0f;
    public static float fakeCameraAspect = 16.0f / 9.0f;




    public static OVRComposition currentComposition = null;





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



internal static class OVRMixedReality
{
    public static bool useFakeExternalCamera = false;

    public static Vector3 fakeCameraFloorLevelPosition = new Vector3(0.0f, 2.0f, -0.5f);
    public static Vector3 fakeCameraEyeLevelPosition = fakeCameraFloorLevelPosition - new Vector3(0.0f, 1.8f, 0.0f);

    public static Quaternion fakeCameraRotation = Quaternion.LookRotation(
        (new Vector3(0.0f, fakeCameraFloorLevelPosition.y, 0.0f) - fakeCameraFloorLevelPosition).normalized,
        Vector3.up);

    public static float fakeCameraFov = 60.0f;
    public static float fakeCameraAspect = 16.0f / 9.0f;

    public static OVRComposition currentComposition = null;

    private static Dictionary<OVRManager.MrcCameraType, GameObject> cameraObjectPool = new Dictionary<OVRManager.MrcCameraType, GameObject>();

    static OVRMixedReality()
    {
        // Initialize object pool
        cameraObjectPool[OVRManager.MrcCameraType.Background] = null;
        cameraObjectPool[OVRManager.MrcCameraType.Foreground] = null;
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
                currentComposition = new OVRExternalComposition(parentObject, mainCamera, configuration, cameraObjectPool);
            }
        }
        else
        {
            Debug.LogError("Unknown/Unsupported CompositionMethod : " + configuration.compositionMethod);
            return;
        }

        currentComposition.Update(parentObject, mainCamera, configuration, trackingOrigin);
    }
}

public class OVRExternalComposition : OVRComposition
{
    public OVRExternalComposition(GameObject parentObject, Camera mainCamera,
        OVRMixedRealityCaptureConfiguration configuration, Dictionary<OVRManager.MrcCameraType, GameObject> cameraObjectPool)
        : base(parentObject, mainCamera, configuration)
    {
        RefreshCameraObjects(parentObject, mainCamera, configuration, cameraObjectPool);
    }

    private void RefreshCameraObjects(GameObject parentObject, Camera mainCamera,
        OVRMixedRealityCaptureConfiguration configuration, Dictionary<OVRManager.MrcCameraType, GameObject> cameraObjectPool)
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

            if (cameraObjectPool[OVRManager.MrcCameraType.Background] != null)
            {
                backgroundCameraGameObject = cameraObjectPool[OVRManager.MrcCameraType.Background];
                cameraObjectPool[OVRManager.MrcCameraType.Background] = null;
            }
            else
            {
                backgroundCameraGameObject = InstantiateCameraObject(mainCamera, OVRManager.MrcCameraType.Background, configuration);
            }

            if (cameraObjectPool[OVRManager.MrcCameraType.Foreground] != null)
            {
                foregroundCameraGameObject = cameraObjectPool[OVRManager.MrcCameraType.Foreground];
                cameraObjectPool[OVRManager.MrcCameraType.Foreground] = null;
            }
            else
            {
                foregroundCameraGameObject = InstantiateCameraObject(mainCamera, OVRManager.MrcCameraType.Foreground, configuration);
            }

            previousMainCameraObject = mainCamera.gameObject;
        }
    }

    private GameObject InstantiateCameraObject(Camera mainCamera, OVRManager.MrcCameraType cameraType, OVRMixedRealityCaptureConfiguration configuration)
    {
        if (configuration.instantiateMixedRealityCameraGameObject != null)
        {
            return configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject, cameraType);
        }
        else
        {
            return Object.Instantiate(mainCamera.gameObject);
        }
    }
}



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
