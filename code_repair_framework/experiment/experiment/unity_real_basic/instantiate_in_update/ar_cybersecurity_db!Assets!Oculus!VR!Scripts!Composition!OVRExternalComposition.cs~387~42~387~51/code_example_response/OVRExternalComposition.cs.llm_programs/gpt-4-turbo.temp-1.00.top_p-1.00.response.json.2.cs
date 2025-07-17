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
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

#if USING_URP
using UnityEngine.Rendering.Universal;
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_ANDROID

public class OVRExternalComposition : OVRComposition
{
private GameObject previousMainCameraObject;
private GameObject foregroundCameraGameObject;
private Camera foregroundCamera;
private GameObject backgroundCameraGameObject;
private Camera backgroundCamera;
#if OVR_ANDROID_MRC
    private bool skipFrame = false;
    private float fpsThreshold = 80.0f;
    private bool isFrameSkipped = true;
    private bool renderCombinedFrame = false;
    private AudioListener audioListener;
    private OVRMRAudioFilter audioFilter;
    private RenderTexture[] mrcRenderTextureArray = new RenderTexture[2];
    private int frameIndex;
    private int lastMrcEncodeFrameSyncId;


    private RenderTexture[] mrcForegroundRenderTextureArray = new RenderTexture[2];


    private double[] cameraPoseTimeArray = new double[2];
#endif

    public override OVRManager.CompositionMethod CompositionMethod()
    {
        return OVRManager.CompositionMethod.External;
    }

    public OVRExternalComposition(GameObject parentObject, Camera mainCamera,
        OVRMixedRealityCaptureConfiguration configuration)
        : base(parentObject, mainCamera, configuration)
    {
#if OVR_ANDROID_MRC
        renderCombinedFrame = false;

        int frameWidth;
        int frameHeight;
        OVRPlugin.Media.GetMrcFrameSize(out frameWidth, out frameHeight);
        Debug.LogFormat("[OVRExternalComposition] Create render texture {0}, {1}",
            renderCombinedFrame ? frameWidth : frameWidth / 2, frameHeight);
        for (int i = 0; i < 2; ++i)
        {
            mrcRenderTextureArray[i] = new RenderTexture(renderCombinedFrame ? frameWidth : frameWidth / 2, frameHeight,
                24, RenderTextureFormat.ARGB32);
            mrcRenderTextureArray[i].Create();
            cameraPoseTimeArray[i] = 0.0;
        }

        skipFrame = OVRManager.display.displayFrequency > fpsThreshold;
        OVRManager.DisplayRefreshRateChanged += DisplayRefreshRateChanged;
        frameIndex = 0;
        lastMrcEncodeFrameSyncId = -1;

        if (!renderCombinedFrame)
        {
            Debug.LogFormat("[OVRExternalComposition] Create extra render textures for foreground");
            for (int i = 0; i < 2; ++i)
            {
                mrcForegroundRenderTextureArray[i] =
                    new RenderTexture(frameWidth / 2, frameHeight, 24, RenderTextureFormat.ARGB32);
                mrcForegroundRenderTextureArray[i].Create();
            }
        }
#endif
        RefreshCameraObjects(parentObject, mainCamera, configuration);
    }


//    private void RefreshCameraObjects(GameObject parentObject, Camera mainCamera,
//        OVRMixedRealityCaptureConfiguration configuration)
//    {
//        if (mainCamera.gameObject != previousMainCameraObject)
//        {
//            Debug.LogFormat("[OVRExternalComposition] Camera refreshed. Rebind camera to {0}",
//                mainCamera.gameObject.name);
//
//            OVRCompositionUtil.SafeDestroy(ref backgroundCameraGameObject);
//            backgroundCamera = null;
//            OVRCompositionUtil.SafeDestroy(ref foregroundCameraGameObject);
//            foregroundCamera = null;
//
//            RefreshCameraRig(parentObject, mainCamera);
//
//            Debug.Assert(backgroundCameraGameObject == null);
//            if (configuration.instantiateMixedRealityCameraGameObject != null)
//            {
//                backgroundCameraGameObject =
//                    configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject,
//                        OVRManager.MrcCameraType.Background);
//            }
//            else
//            {
//                backgroundCameraGameObject = Object.Instantiate(mainCamera.gameObject);
//            }
//
//            backgroundCameraGameObject.name = "OculusMRC_BackgroundCamera";
//            backgroundCameraGameObject.transform.parent =
//                cameraInTrackingSpace ? cameraRig.trackingSpace : parentObject.transform;
//            if (backgroundCameraGameObject.GetComponent<AudioListener>())
//            {
//                Object.Destroy(backgroundCameraGameObject.GetComponent<AudioListener>());
//            }
//
//            if (backgroundCameraGameObject.GetComponent<OVRManager>())
//            {
//                Object.Destroy(backgroundCameraGameObject.GetComponent<OVRManager>());
//            }
//
//            backgroundCamera = backgroundCameraGameObject.GetComponent<Camera>();
//            backgroundCamera.tag = "Untagged";
//#if USING_MRC_COMPATIBLE_URP_VERSION
//            var backgroundCamData = backgroundCamera.GetUniversalAdditionalCameraData();
//            if (backgroundCamData != null)
//            {
//                backgroundCamData.allowXRRendering = false;
//            }
//#elif USING_URP
//            Debug.LogError("Using URP with MRC is only supported with URP version 10.0.0 or higher. Consider using Unity 2020 or higher.");
//#else
//            backgroundCamera.stereoTargetEye = StereoTargetEyeMask.None;
//#endif
//            backgroundCamera.depth = 99990.0f;
//            backgroundCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
//            backgroundCamera.cullingMask = (backgroundCamera.cullingMask & ~configuration.extraHiddenLayers) |
//                                           configuration.extraVisibleLayers;
//#if OVR_ANDROID_MRC
//            backgroundCamera.targetTexture = mrcRenderTextureArray[0];
//            if (!renderCombinedFrame)
//            {
//                backgroundCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
//            }
//#endif
//
//            Debug.Assert(foregroundCameraGameObject == null);
//            if (configuration.instantiateMixedRealityCameraGameObject != null)
//            {
//                foregroundCameraGameObject =
//                    configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject,
//                        OVRManager.MrcCameraType.Foreground);
//            }
//            else
//            {
//                foregroundCameraGameObject = Object.Instantiate(mainCamera.gameObject);
//            }
//
//            foregroundCameraGameObject.name = "OculusMRC_ForgroundCamera";
//            foregroundCameraGameObject.transform.parent =
//                cameraInTrackingSpace ? cameraRig.trackingSpace : parentObject.transform;
//            if (foregroundCameraGameObject.GetComponent<AudioListener>())
//            {
//                Object.Destroy(foregroundCameraGameObject.GetComponent<AudioListener>());
//            }
//
//            if (foregroundCameraGameObject.GetComponent<OVRManager>())
//            {
//                Object.Destroy(foregroundCameraGameObject.GetComponent<OVRManager>());
//            }
//
//            foregroundCamera = foregroundCameraGameObject.GetComponent<Camera>();
//            foregroundCamera.tag = "Untagged";
//#if USING_MRC_COMPATIBLE_URP_VERSION
//            var foregroundCamData = foregroundCamera.GetUniversalAdditionalCameraData();
//            if (foregroundCamData != null)
//            {
//                foregroundCamData.allowXRRendering = false;
//            }
//#elif USING_URP
//            Debug.LogError("Using URP with MRC is only supported with URP version 10.0.0 or higher. Consider using Unity 2020 or higher.");
//#else
//            foregroundCamera.stereoTargetEye = StereoTargetEyeMask.None;
//#endif
//            foregroundCamera.depth =
//                backgroundCamera.depth + 1.0f; // enforce the forground be rendered after the background
//            foregroundCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
//            foregroundCamera.clearFlags = CameraClearFlags.Color;
//#if OVR_ANDROID_MRC
//            foregroundCamera.backgroundColor = configuration.externalCompositionBackdropColorQuest;
//#else
//            foregroundCamera.backgroundColor = configuration.externalCompositionBackdropColorRift;
//#endif
//            foregroundCamera.cullingMask = (foregroundCamera.cullingMask & ~configuration.extraHiddenLayers) |
//                                           configuration.extraVisibleLayers;
//
//#if OVR_ANDROID_MRC
//            if (renderCombinedFrame)
//            {
//                foregroundCamera.targetTexture = mrcRenderTextureArray[0];
//            }
//            else
//            {
//                foregroundCamera.targetTexture = mrcForegroundRenderTextureArray[0];
//                foregroundCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
//            }
//#endif
//
//
//
//
//
//
//
//
//            previousMainCameraObject = mainCamera.gameObject;
//        }
//    }


#if OVR_ANDROID_MRC
    private void RefreshAudioFilter(Camera mainCamera)
    {
        if (audioListener == null || !audioListener.enabled || !audioListener.gameObject.activeInHierarchy)
        {
            CleanupAudioFilter();


            AudioListener tmpAudioListener = cameraRig != null && cameraRig.centerEyeAnchor.gameObject.activeInHierarchy
                ? cameraRig.centerEyeAnchor.GetComponent<AudioListener>()
                : null;


            if (tmpAudioListener == null || !tmpAudioListener.enabled)
            {
                tmpAudioListener = mainCamera != null && mainCamera.gameObject.activeInHierarchy
                    ? mainCamera.GetComponent<AudioListener>()
                    : null;
            }


            if (tmpAudioListener == null || !tmpAudioListener.enabled)
            {
                mainCamera = Camera.main;
                tmpAudioListener = mainCamera != null && mainCamera.gameObject.activeInHierarchy
                    ? mainCamera.GetComponent<AudioListener>()
                    : null;
            }


            if (tmpAudioListener == null || !tmpAudioListener.enabled)
            {
                Object[] allListeners = Object.FindObjectsOfType<AudioListener>();
                foreach (var l in allListeners)
                {
                    AudioListener al = l as AudioListener;
                    if (al != null && al.enabled && al.gameObject.activeInHierarchy)
                    {
                        tmpAudioListener = al;
                        break;
                    }
                }
            }

            if (tmpAudioListener == null || !tmpAudioListener.enabled)
            {
                Debug.LogWarning("[OVRExternalComposition] No AudioListener in scene");
            }
            else
            {
                Debug.LogFormat("[OVRExternalComposition] AudioListener found, obj {0}",
                    tmpAudioListener.gameObject.name);
                audioListener = tmpAudioListener;
                audioFilter = audioListener.gameObject.AddComponent<OVRMRAudioFilter>();
                audioFilter.composition = this;
                Debug.LogFormat("OVRMRAudioFilter added");
            }
        }
    }

    private float[] cachedAudioDataArray = null;

    private int CastMrcFrame(int castTextureIndex)
    {
        int audioFrames;
        int audioChannels;
        GetAndResetAudioData(ref cachedAudioDataArray, out audioFrames, out audioChannels);

        int syncId = -1;

        bool ret = false;
        if (OVRPlugin.Media.GetMrcInputVideoBufferType() == OVRPlugin.Media.InputVideoBufferType.TextureHandle)
        {
            ret = OVRPlugin.Media.EncodeMrcFrame(mrcRenderTextureArray[castTextureIndex].GetNativeTexturePtr(),
                renderCombinedFrame
                    ? System.IntPtr.Zero
                    : mrcForegroundRenderTextureArray[castTextureIndex].GetNativeTexturePtr(),
                cachedAudioDataArray, audioFrames, audioChannels, AudioSettings.dspTime,
                cameraPoseTimeArray[castTextureIndex], ref syncId);
        }
        else
        {
            ret = OVRPlugin.Media.EncodeMrcFrame(mrcRenderTextureArray[castTextureIndex], cachedAudioDataArray,
                audioFrames, audioChannels, AudioSettings.dspTime, cameraPoseTimeArray[castTextureIndex], ref syncId);
        }

        if (!ret)
        {
            Debug.LogWarning("EncodeMrcFrame failed. Likely caused by OBS plugin disconnection");
            return -1;
        }

        return syncId;
    }

    private void SetCameraTargetTexture(int drawTextureIndex)
    {
        if (renderCombinedFrame)
        {
            RenderTexture texture = mrcRenderTextureArray[drawTextureIndex];
            if (backgroundCamera.targetTexture != texture)
            {
                backgroundCamera.targetTexture = texture;
            }

            if (foregroundCamera.targetTexture != texture)
            {
                foregroundCamera.targetTexture = texture;
            }
        }
        else
        {
            RenderTexture bgTexture = mrcRenderTextureArray[drawTextureIndex];
            RenderTexture fgTexture = mrcForegroundRenderTextureArray[drawTextureIndex];
            if (backgroundCamera.targetTexture != bgTexture)
            {
                backgroundCamera.targetTexture = bgTexture;
            }

            if (foregroundCamera.targetTexture != fgTexture)
            {
                foregroundCamera.targetTexture = fgTexture;
            }
        }
    }
#endif




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
            // Move Object.Instantiate() call outside of Update, to a separate setup method
            SetupBackgroundCamera(mainCamera, configuration);
        }

        SetupBackgroundCameraSettings(parentObject, mainCamera, configuration);

        Debug.Assert(foregroundCameraGameObject == null);
        if (configuration.instantiateMixedRealityCameraGameObject != null)
        {
            foregroundCameraGameObject =
                configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject,
                    OVRManager.MrcCameraType.Foreground);
        }
        else
        {
            // Move Object.Instantiate() call outside of Update, to a separate setup method
            SetupForegroundCamera(mainCamera, configuration);
        }

        SetupForegroundCameraSettings(parentObject, mainCamera, configuration);

        previousMainCameraObject = mainCamera.gameObject;
    }
}

private void SetupBackgroundCamera(Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration)
{
    backgroundCameraGameObject = Object.Instantiate(mainCamera.gameObject);
}

private void SetupBackgroundCameraSettings(GameObject parentObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration)
{
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
}

private void SetupForegroundCamera(Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration)
{
    foregroundCameraGameObject = Object.Instantiate(mainCamera.gameObject);
}

private void SetupForegroundCameraSettings(GameObject parentObject, Camera mainCamera, OVRMixedRealityCaptureConfiguration configuration)
{
    foregroundCameraGameObject.name = "OculusMRC_ForgroundCamera";
    foregroundCameraGameObject.transform.parent =
        cameraInTrackingSpace ? cameraRig.trackingSpace : parentObject.transform;
    if (foregroundCameraGameObject.GetComponent<AudioListener>())
    {
        Object.Destroy(foregroundCameraGameObject.GetComponent<AudioListener>());
    }

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
    foregroundCamera.depth = backgroundCamera.depth + 1.0f; // enforce the foreground be rendered after the background
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
}



#if OVR_ANDROID_MRC
    private void CleanupAudioFilter()
    {
        if (audioFilter)
        {
            audioFilter.composition = null;
            Object.Destroy(audioFilter);
            Debug.LogFormat("OVRMRAudioFilter destroyed");
            audioFilter = null;
        }
    }
#endif

    public override void Cleanup()
    {
        OVRCompositionUtil.SafeDestroy(ref backgroundCameraGameObject);
        backgroundCamera = null;
        OVRCompositionUtil.SafeDestroy(ref foregroundCameraGameObject);
        foregroundCamera = null;
        Debug.Log("ExternalComposition deactivated");

#if OVR_ANDROID_MRC
        if (lastMrcEncodeFrameSyncId != -1)
        {
            OVRPlugin.Media.SyncMrcFrame(lastMrcEncodeFrameSyncId);
            lastMrcEncodeFrameSyncId = -1;
        }

        CleanupAudioFilter();

        for (int i = 0; i < 2; ++i)
        {
            mrcRenderTextureArray[i].Release();
            mrcRenderTextureArray[i] = null;

            if (!renderCombinedFrame)
            {
                mrcForegroundRenderTextureArray[i].Release();
                mrcForegroundRenderTextureArray[i] = null;
            }
        }

        OVRManager.DisplayRefreshRateChanged -= DisplayRefreshRateChanged;
        frameIndex = 0;
#endif
    }

    private readonly object audioDataLock = new object();
    private List<float> cachedAudioData = new List<float>(16384);
    private int cachedChannels = 0;

    public void CacheAudioData(float[] data, int channels)
    {
        lock (audioDataLock)
        {
            if (channels != cachedChannels)
            {
                cachedAudioData.Clear();
            }

            cachedChannels = channels;
            cachedAudioData.AddRange(data);
            //Debug.LogFormat("[CacheAudioData] dspTime {0} indata {1} channels {2} accu_len {3}", AudioSettings.dspTime, data.Length, channels, cachedAudioData.Count);
        }
    }

    public void GetAndResetAudioData(ref float[] audioData, out int audioFrames, out int channels)
    {
        lock (audioDataLock)
        {
            //Debug.LogFormat("[GetAndResetAudioData] dspTime {0} accu_len {1}", AudioSettings.dspTime, cachedAudioData.Count);
            if (audioData == null || audioData.Length < cachedAudioData.Count)
            {
                audioData = new float[cachedAudioData.Capacity];
            }

            cachedAudioData.CopyTo(audioData);
            audioFrames = cachedAudioData.Count;
            channels = cachedChannels;
            cachedAudioData.Clear();
        }
    }

#if OVR_ANDROID_MRC
    private void DisplayRefreshRateChanged(float fromRefreshRate, float toRefreshRate)
    {
        skipFrame = toRefreshRate > fpsThreshold;
    }
#endif
}

#if OVR_ANDROID_MRC
public class OVRMRAudioFilter : MonoBehaviour
{
    private bool running = false;

    public OVRExternalComposition composition;

    void Start()
    {
        running = true;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        if (composition != null)
        {
            composition.CacheAudioData(data, channels);
        }
    }
}
#endif

#endif
