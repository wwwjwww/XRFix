
	private void EnableRenderers()
	{
		_OpaqueMeshRenderer.enabled = _OpaqueVignetteVisible;
		_TransparentMeshRenderer.enabled = _TransparentVignetteVisible;
	}

	private void DisableRenderers()
	{
		_OpaqueMeshRenderer.enabled = false;
		_TransparentMeshRenderer.enabled = false;
	}

	// Objects are enabled on pre cull and disabled on post render so they only draw in this camera
	private void OnPreCull()
	{
		EnableRenderers();
	}

	private void OnPostRender()
	{
		DisableRenderers();
	}

#if UNITY_2019_1_OR_NEWER
	private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
#else
	private void OnBeginCameraRendering(Camera camera)
#endif
	{
		if (camera == _Camera)
		{
			EnableRenderers();
		}
		else
		{
			DisableRenderers();
		}
	}
}
