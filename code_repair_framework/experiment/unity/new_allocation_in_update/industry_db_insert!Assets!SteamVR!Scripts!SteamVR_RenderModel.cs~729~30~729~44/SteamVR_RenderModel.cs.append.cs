
        Dictionary<int, string> nameCache;

        public void UpdateComponents(CVRRenderModels renderModels)
        {
            if (renderModels == null)
                return;

            if (transform.childCount == 0)
                return;

            if (nameCache == null)
                nameCache = new Dictionary<int, string>();

            for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
            {
                Transform child = transform.GetChild(childIndex);

                // Cache names since accessing an object's name allocates memory.
                string componentName;
                if (!nameCache.TryGetValue(child.GetInstanceID(), out componentName))
                {
                    componentName = child.name;
                    nameCache.Add(child.GetInstanceID(), componentName);
                }

                var componentState = new RenderModel_ComponentState_t();
                if (!renderModels.GetComponentStateForDevicePath(renderModelName, componentName, SteamVR_Input_Source.GetHandle(inputSource), ref controllerModeState, ref componentState))
                    continue;

                child.localPosition = componentState.mTrackingToComponentRenderModel.GetPosition();
                child.localRotation = componentState.mTrackingToComponentRenderModel.GetRotation();

                Transform attach = null;
                for (int childChildIndex = 0; childChildIndex < child.childCount; childChildIndex++)
                {
                    Transform childChild = child.GetChild(childChildIndex);
                    int childInstanceID = childChild.GetInstanceID();
                    string childName;
                    if (!nameCache.TryGetValue(childInstanceID, out childName))
                    {
                        childName = childChild.name;
                        nameCache.Add(childInstanceID, componentName);
                    }

                    if (childName == SteamVR_RenderModel.k_localTransformName)
                        attach = childChild;
                }

                if (attach != null)
                {
                    attach.position = transform.TransformPoint(componentState.mTrackingToComponentLocal.GetPosition());
                    attach.rotation = transform.rotation * componentState.mTrackingToComponentLocal.GetRotation();

                    initializedAttachPoints = true;
                }

                bool visible = (componentState.uProperties & (uint)EVRComponentProperty.IsVisible) != 0;
                if (visible != child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(visible);
                }
            }
        }

        public void SetDeviceIndex(int newIndex)
        {
            this.index = (SteamVR_TrackedObject.EIndex)newIndex;

            modelOverride = "";

            if (enabled)
            {
                UpdateModel();
            }
        }

        public void SetInputSource(SteamVR_Input_Sources newInputSource)
        {
            inputSource = newInputSource;
        }

        private static void Sleep()
        {
#if !UNITY_METRO
            //System.Threading.Thread.SpinWait(1); //faster napping
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
}