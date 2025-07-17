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



        private static ObjectPool objPool;

        public static ObjectPool GetInstance()
        {
            if (!objPool)
            {
                objPool = new ObjectPool();
            }
    
            return objPool;
        }

        private Dictionary<string, GameObject> _objPool = new Dictionary<string, GameObject>();

        private void Awake()
        {
            foreach (GameObject go in GetCommentsFromResources("OVRMixedReality"))
            {
                _objPool.Add(go.name, go);
            }
        }

        private List<GameObject> GetCommentsFromResources(string resourceDirectory)
        {
            var result = new List<GameObject>();
            var filePath = System.IO.Path.Combine(Application.dataPath, "Resources");
            if (!Directory.Exists(filePath))
            {
                return result;
            }
            Directory.GetDirectories(filePath, resourceDirectory, SearchOption.AllDirectories);
            var files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories);
            if (files == null)
            {
                return result;
            }
    
            foreach (string file in files)
            {
                var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                if (gameObject != null)
                {
                    result.Add(gameObject);
                }
            }
            return result;
        }

        public void Request(string objName)
        {
            if (_objPool.ContainsKey(objName))
            {
                var obj = _objPool[objName];
                if (objPool is null)
                {
                    objPool = new ObjectPool();
                }
    
                if (!objPool._objPool.ContainsKey(obj.name))
                {
                    objPool._objPool.Add(obj.name, obj);
                }
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
                var objPool = ObjectPool.GetInstance();
                var cameraObj = objPool.Request(configuration.externalCameraPrefabName);
                if (cameraObj != null)
                {
                    currentComposition = new OVRExternalComposition(parentObject, mainCamera, cameraObj, configuration);
                }
            }
        }
        else
        {
            Debug.LogError("Unknown/Unsupported CompositionMethod : " + configuration.compositionMethod);
            return;
        }
    
        currentComposition.Update(parentObject, mainCamera, configuration, trackingOrigin);
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
