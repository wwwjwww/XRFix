
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


