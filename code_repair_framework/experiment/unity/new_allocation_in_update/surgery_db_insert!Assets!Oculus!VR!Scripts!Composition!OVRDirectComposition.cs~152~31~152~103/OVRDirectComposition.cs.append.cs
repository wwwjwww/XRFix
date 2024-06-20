
	public override void Cleanup()
	{
		base.Cleanup();

		OVRCompositionUtil.SafeDestroy(ref directCompositionCameraGameObject);
		directCompositionCamera = null;

		Debug.Log("DirectComposition deactivated");
	}
}

#endif
