
#if OVR_ANDROID_MRC
    private void RefreshAudioFilter(Camera mainCamera)
    {
        if (audioListener == null || !audioListener.enabled || !audioListener.gameObject.activeInHierarchy)
        {
            CleanupAudioFilter();

            // first try cameraRig
            AudioListener tmpAudioListener = cameraRig != null && cameraRig.centerEyeAnchor.gameObject.activeInHierarchy
                ? cameraRig.centerEyeAnchor.GetComponent<AudioListener>()
                : null;

            // second try mainCamera
            if (tmpAudioListener == null || !tmpAudioListener.enabled)
            {
                tmpAudioListener = mainCamera != null && mainCamera.gameObject.activeInHierarchy
                    ? mainCamera.GetComponent<AudioListener>()
                    : null;
            }

            // third try Camera.main (expensive)
            if (tmpAudioListener == null || !tmpAudioListener.enabled)
            {
                mainCamera = Camera.main;
                tmpAudioListener = mainCamera != null && mainCamera.gameObject.activeInHierarchy
                    ? mainCamera.GetComponent<AudioListener>()
                    : null;
            }

            // fourth, search for all AudioListeners (very expensive)
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
        //Debug.Log("EncodeFrameThreadObject EncodeMrcFrame");
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


    public override void Update(GameObject gameObject, Camera mainCamera,
        OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
    {
#if OVR_ANDROID_MRC
        if (skipFrame && OVRPlugin.Media.IsCastingToRemoteClient())
        {
            isFrameSkipped = !isFrameSkipped;
            if (isFrameSkipped)
            {
                return;
            }
        }
#endif

        RefreshCameraObjects(gameObject, mainCamera, configuration);

        // the HandNodePoseStateLatency doesn't apply to the external composition. Always enforce it to 0.0
        OVRPlugin.SetHandNodePoseStateLatency(0.0);

        // For third-person camera to use for calculating camera position with different anchors
        OVRPose stageToLocalPose =
            OVRPlugin.GetTrackingTransformRelativePose(OVRPlugin.TrackingOrigin.Stage).ToOVRPose();
        OVRPose localToStagePose = stageToLocalPose.Inverse();
        OVRPose head = localToStagePose * OVRPlugin.GetNodePose(OVRPlugin.Node.Head, OVRPlugin.Step.Render).ToOVRPose();
        OVRPose leftC = localToStagePose *
                        OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, OVRPlugin.Step.Render).ToOVRPose();
        OVRPose rightC = localToStagePose *
                         OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, OVRPlugin.Step.Render).ToOVRPose();
        OVRPlugin.Media.SetMrcHeadsetControllerPose(head.ToPosef(), leftC.ToPosef(), rightC.ToPosef());

#if OVR_ANDROID_MRC
        RefreshAudioFilter(mainCamera);

        int drawTextureIndex = (frameIndex / 2) % 2;
        int castTextureIndex = 1 - drawTextureIndex;

        backgroundCamera.enabled = (frameIndex % 2) == 0;
        foregroundCamera.enabled = (frameIndex % 2) == 1;

        if (frameIndex % 2 == 0)
        {
            if (lastMrcEncodeFrameSyncId != -1)
            {
                OVRPlugin.Media.SyncMrcFrame(lastMrcEncodeFrameSyncId);
                lastMrcEncodeFrameSyncId = -1;
            }

            lastMrcEncodeFrameSyncId = CastMrcFrame(castTextureIndex);
            SetCameraTargetTexture(drawTextureIndex);
        }

        ++frameIndex;
#endif

        backgroundCamera.clearFlags = mainCamera.clearFlags;
        backgroundCamera.backgroundColor = mainCamera.backgroundColor;
        if (configuration.dynamicCullingMask)
        {
            backgroundCamera.cullingMask = (mainCamera.cullingMask & ~configuration.extraHiddenLayers) |
                                           configuration.extraVisibleLayers;
        }

        backgroundCamera.nearClipPlane = mainCamera.nearClipPlane;
        backgroundCamera.farClipPlane = mainCamera.farClipPlane;

        if (configuration.dynamicCullingMask)
        {
            foregroundCamera.cullingMask = (mainCamera.cullingMask & ~configuration.extraHiddenLayers) |
                                           configuration.extraVisibleLayers;
        }

        foregroundCamera.nearClipPlane = mainCamera.nearClipPlane;
        foregroundCamera.farClipPlane = mainCamera.farClipPlane;

        if (OVRMixedReality.useFakeExternalCamera || OVRPlugin.GetExternalCameraCount() == 0)
        {
            OVRPose worldSpacePose = new OVRPose();
            OVRPose trackingSpacePose = new OVRPose();
            trackingSpacePose.position = trackingOrigin == OVRManager.TrackingOrigin.EyeLevel
                ? OVRMixedReality.fakeCameraEyeLevelPosition
                : OVRMixedReality.fakeCameraFloorLevelPosition;
            trackingSpacePose.orientation = OVRMixedReality.fakeCameraRotation;
            worldSpacePose = OVRExtensions.ToWorldSpacePose(trackingSpacePose, mainCamera);

            backgroundCamera.fieldOfView = OVRMixedReality.fakeCameraFov;
            backgroundCamera.aspect = OVRMixedReality.fakeCameraAspect;
            foregroundCamera.fieldOfView = OVRMixedReality.fakeCameraFov;
            foregroundCamera.aspect = OVRMixedReality.fakeCameraAspect;

            if (cameraInTrackingSpace)
            {
                backgroundCamera.transform.FromOVRPose(trackingSpacePose, true);
                foregroundCamera.transform.FromOVRPose(trackingSpacePose, true);
            }
            else
            {
                backgroundCamera.transform.FromOVRPose(worldSpacePose);
                foregroundCamera.transform.FromOVRPose(worldSpacePose);
            }
        }
        else
        {
            OVRPlugin.CameraExtrinsics extrinsics;
            OVRPlugin.CameraIntrinsics intrinsics;

            // So far, only support 1 camera for MR and always use camera index 0
            if (OVRPlugin.GetMixedRealityCameraInfo(0, out extrinsics, out intrinsics))
            {
                float fovY = Mathf.Atan(intrinsics.FOVPort.UpTan) * Mathf.Rad2Deg * 2;
                float aspect = intrinsics.FOVPort.LeftTan / intrinsics.FOVPort.UpTan;
                backgroundCamera.fieldOfView = fovY;
                backgroundCamera.aspect = aspect;
                foregroundCamera.fieldOfView = fovY;
                foregroundCamera.aspect = intrinsics.FOVPort.LeftTan / intrinsics.FOVPort.UpTan;

                if (cameraInTrackingSpace)
                {
                    OVRPose trackingSpacePose = ComputeCameraTrackingSpacePose(extrinsics);
                    backgroundCamera.transform.FromOVRPose(trackingSpacePose, true);
                    foregroundCamera.transform.FromOVRPose(trackingSpacePose, true);
                }
                else
                {
                    OVRPose worldSpacePose = ComputeCameraWorldSpacePose(extrinsics, mainCamera);
                    backgroundCamera.transform.FromOVRPose(worldSpacePose);
                    foregroundCamera.transform.FromOVRPose(worldSpacePose);
                }
#if OVR_ANDROID_MRC
                cameraPoseTimeArray[drawTextureIndex] = extrinsics.LastChangedTimeSeconds;
#endif
            }
            else
            {
                Debug.LogError("Failed to get external camera information");
                return;
            }
        }

        Vector3 headToExternalCameraVec = mainCamera.transform.position - foregroundCamera.transform.position;
        float clipDistance = Vector3.Dot(headToExternalCameraVec, foregroundCamera.transform.forward);
        foregroundCamera.farClipPlane = Mathf.Max(foregroundCamera.nearClipPlane + 0.001f, clipDistance);
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
