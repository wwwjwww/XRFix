
	void OnApplicationPause()
	{
#if OVR_ANDROID_MRC
		OVRPlugin.Media.SetMrcFrameImageFlipped(true);
		OVRPlugin.Media.SetPlatformCameraMode(OVRPlugin.Media.PlatformCameraMode.Disabled);
#endif
	}

	void OnApplicationQuit()
	{
#if OVR_ANDROID_MRC
		OVRPlugin.Media.SetMrcFrameImageFlipped(true);
		OVRPlugin.Media.SetPlatformCameraMode(OVRPlugin.Media.PlatformCameraMode.Disabled);
#endif
	}
}
