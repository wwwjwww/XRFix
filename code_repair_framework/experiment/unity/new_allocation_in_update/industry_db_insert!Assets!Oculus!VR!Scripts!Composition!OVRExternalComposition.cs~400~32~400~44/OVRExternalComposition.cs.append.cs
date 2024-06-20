
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

		for (int i=0; i<2; ++i)
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
		lock(audioDataLock)
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
		lock(audioDataLock)
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
