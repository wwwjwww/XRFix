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
    private GameObject previousMainCameraObject = null;
    public GameObject foregroundCameraGameObject = null;
    public Camera foregroundCamera = null;
    public GameObject backgroundCameraGameObject = null;
    public Camera backgroundCamera = null;
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

