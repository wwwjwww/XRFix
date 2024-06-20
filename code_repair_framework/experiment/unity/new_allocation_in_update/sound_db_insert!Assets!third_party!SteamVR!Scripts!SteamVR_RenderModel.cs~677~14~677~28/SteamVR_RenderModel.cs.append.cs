
	Dictionary<int, string> nameCache;

	public void UpdateComponents(CVRRenderModels renderModels)
	{
		if (renderModels == null)
			return;

		var t = transform;
		if (t.childCount == 0)
			return;

		var controllerState = (index != SteamVR_TrackedObject.EIndex.None) ?
			SteamVR_Controller.Input((int)index).GetState() : new VRControllerState_t();

		if (nameCache == null)
			nameCache = new Dictionary<int, string>();

		for (int i = 0; i < t.childCount; i++)
		{
			var child = t.GetChild(i);

			// Cache names since accessing an object's name allocate memory.
			string name;
			if (!nameCache.TryGetValue(child.GetInstanceID(), out name))
			{
				name = child.name;
				nameCache.Add(child.GetInstanceID(), name);
            }

			var componentState = new RenderModel_ComponentState_t();
            if (!renderModels.GetComponentState(renderModelName, name, ref controllerState, ref controllerModeState, ref componentState))
				continue;

			var componentTransform = new SteamVR_Utils.RigidTransform(componentState.mTrackingToComponentRenderModel);
			child.localPosition = componentTransform.pos;
			child.localRotation = componentTransform.rot;

			var attach = child.Find(k_localTransformName);
			if (attach != null)
			{
				var attachTransform = new SteamVR_Utils.RigidTransform(componentState.mTrackingToComponentLocal);
				attach.position = t.TransformPoint(attachTransform.pos);
				attach.rotation = t.rotation * attachTransform.rot;
			}

			bool visible = (componentState.uProperties & (uint)EVRComponentProperty.IsVisible) != 0;
			if (visible != child.gameObject.activeSelf)
			{
				child.gameObject.SetActive(visible);
			}
		}
	}

	public void SetDeviceIndex(int index)
	{
		this.index = (SteamVR_TrackedObject.EIndex)index;
		modelOverride = "";

		if (enabled)
		{
			UpdateModel();
		}
	}

	private static void Sleep()
	{
#if !UNITY_METRO
		System.Threading.Thread.Sleep(1);
#endif
	}

    /// <summary>
    /// Helper function to handle the inconvenient fact that the packing for RenderModel_t is 
    /// different on Linux/OSX (4) than it is on Windows (8)
    /// </summary>
    /// <param name="pRenderModel">native pointer to the RenderModel_t</param>
    /// <returns></returns>
    private RenderModel_t MarshalRenderModel(System.IntPtr pRenderModel)
    {
        if ((System.Environment.OSVersion.Platform == System.PlatformID.MacOSX) ||
            (System.Environment.OSVersion.Platform == System.PlatformID.Unix))
        {
            var packedModel = (RenderModel_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t_Packed));
            RenderModel_t model = new RenderModel_t();
            packedModel.Unpack(ref model);
            return model;
        }
        else
        {
            return (RenderModel_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t));
        }
    }

    /// <summary>
    /// Helper function to handle the inconvenient fact that the packing for RenderModel_TextureMap_t is 
    /// different on Linux/OSX (4) than it is on Windows (8)
    /// </summary>
    /// <param name="pRenderModel">native pointer to the RenderModel_TextureMap_t</param>
    /// <returns></returns>
    private RenderModel_TextureMap_t MarshalRenderModel_TextureMap(System.IntPtr pRenderModel)
    {
        if ((System.Environment.OSVersion.Platform == System.PlatformID.MacOSX) ||
            (System.Environment.OSVersion.Platform == System.PlatformID.Unix))
        {
            var packedModel = (RenderModel_TextureMap_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t_Packed));
            RenderModel_TextureMap_t model = new RenderModel_TextureMap_t();
            packedModel.Unpack(ref model);
            return model;
        }
        else
        {
            return (RenderModel_TextureMap_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t));
        }
    }
}

