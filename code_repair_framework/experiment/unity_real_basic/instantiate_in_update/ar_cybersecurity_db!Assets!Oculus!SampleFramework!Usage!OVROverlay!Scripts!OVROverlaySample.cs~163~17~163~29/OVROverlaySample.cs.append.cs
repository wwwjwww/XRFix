
        #endregion

        #region Private Functions

        /// <summary>
        /// Usage: Activate the world geometry and deactivate OVROverlay display
        /// </summary>
        void ActivateWorldGeo()
        {
            worldspaceGeoParent.SetActive(true);
            uiGeoParent.SetActive(false);
            uiCamera.SetActive(false);
            cameraRenderOverlay.enabled = false;
            renderingLabelOverlay.enabled = true;
            renderingLabelOverlay.textures[0] = applicationLabelTexture;
            Debug.Log("Switched to ActivateWorldGeo");
        }

        /// <summary>
        /// Usage: Activate OVROverlay display and deactivate the world geometry
        /// </summary>
        void ActivateOVROverlay()
        {
            worldspaceGeoParent.SetActive(false);
            uiCamera.SetActive(true);
            cameraRenderOverlay.enabled = true;
            uiGeoParent.SetActive(true);
            renderingLabelOverlay.enabled = true;
            renderingLabelOverlay.textures[0] = compositorLabelTexture;
            Debug.Log("Switched to ActivateOVROVerlay");
        }

        /// <summary>
        /// Usage: Deactivate both world geometry and OVROverlay display
        /// </summary>
        void ActivateNone()
        {
            worldspaceGeoParent.SetActive(false);
            uiCamera.SetActive(false);
            cameraRenderOverlay.enabled = false;
            uiGeoParent.SetActive(false);
            renderingLabelOverlay.enabled = false;
            Debug.Log("Switched to ActivateNone");
        }


        /// <summary>
        /// This function is to simulate a level load event in Unity
        /// The idea is to enable a cubemap overlay right before any action that will stall the main thread
        /// This cubemap overlay can be combined with other OVROverlay objects, such as animated textures to indicate "Loading..."
        /// </summary>
        void TriggerLoad()
        {
            StartCoroutine(WaitforOVROverlay());
        }

        IEnumerator WaitforOVROverlay()
        {
            Transform camTransform = mainCamera.transform;
            Transform uiTextOverlayTrasnform = loadingTextQuadOverlay.transform;
            Vector3 newPos = camTransform.position + camTransform.forward * distanceFromCamToLoadText;
            newPos.y = camTransform.position.y;
            uiTextOverlayTrasnform.position = newPos;
            cubemapOverlay.enabled = true;
            loadingTextQuadOverlay.enabled = true;
            noneRadioButton.isOn = true;
            yield return new WaitForSeconds(0.1f);
            ClearObjects();
            SimulateLevelLoad();
            cubemapOverlay.enabled = false;
            loadingTextQuadOverlay.enabled = false;
            yield return null;
        }


        /// <summary>
        /// Usage: Destroy all loaded resources and switch back to world geometry rendering mode.
        /// </summary>
        void TriggerUnload()
        {
            ClearObjects();
            applicationRadioButton.isOn = true;
        }

        /// <summary>
        /// Usage: Recreate UI render target according overlay type and overlay size
        /// </summary>
        void CameraAndRenderTargetSetup()
        {
            float overlayWidth = cameraRenderOverlay.transform.localScale.x;
            float overlayHeight = cameraRenderOverlay.transform.localScale.y;
            float overlayRadius = cameraRenderOverlay.transform.localScale.z;

#if UNITY_ANDROID
            // Gear VR display panel resolution
            float hmdPanelResWidth = 2560;
            float hmdPanelResHeight = 1440;
#else
            // Rift display panel resolution
            float hmdPanelResWidth = 2160;
            float hmdPanelResHeight = 1200;
#endif

            float singleEyeScreenPhysicalResX = hmdPanelResWidth * 0.5f;
            float singleEyeScreenPhysicalResY = hmdPanelResHeight;

            // Calculate RT Height
            // screenSizeYInWorld : how much world unity the full screen can cover at overlayQuad's location vertically
            // pixelDensityY: pixels / world unit ( meter )

            float halfFovY = mainCamera.GetComponent<Camera>().fieldOfView / 2;
            float screenSizeYInWorld = 2 * overlayRadius * Mathf.Tan(Mathf.Deg2Rad * halfFovY);
            float pixelDensityYPerWorldUnit = singleEyeScreenPhysicalResY / screenSizeYInWorld;
            float renderTargetHeight = pixelDensityYPerWorldUnit * overlayWidth;

            // Calculate RT width
            float renderTargetWidth = 0.0f;

            // screenSizeXInWorld : how much world unity the full screen can cover at overlayQuad's location horizontally
            // pixelDensityY: pixels / world unit ( meter )

            float screenSizeXInWorld = screenSizeYInWorld * mainCamera.GetComponent<Camera>().aspect;
            float pixelDensityXPerWorldUnit = singleEyeScreenPhysicalResX / screenSizeXInWorld;
            renderTargetWidth = pixelDensityXPerWorldUnit * overlayWidth;

            // Compute the orthographic size for the camera
            float orthographicSize = overlayHeight / 2.0f;
            float orthoCameraAspect = overlayWidth / overlayHeight;
            uiCamera.GetComponent<Camera>().orthographicSize = orthographicSize;
            uiCamera.GetComponent<Camera>().aspect = orthoCameraAspect;

            if (uiCamera.GetComponent<Camera>().targetTexture != null)
                uiCamera.GetComponent<Camera>().targetTexture.Release();

            RenderTexture overlayRT = new RenderTexture(
                (int)renderTargetWidth * 2,
                (int)renderTargetHeight * 2,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.sRGB);
            Debug.Log("Created RT of resolution w: " + renderTargetWidth + " and h: " + renderTargetHeight);

            overlayRT.hideFlags = HideFlags.DontSave;
            overlayRT.useMipMap = true;
            overlayRT.filterMode = FilterMode.Trilinear;
            overlayRT.anisoLevel = 4;
#if UNITY_5_5_OR_NEWER
            overlayRT.autoGenerateMips = true;
#else
        overlayRT.generateMips = true;
#endif
            uiCamera.GetComponent<Camera>().targetTexture = overlayRT;

            cameraRenderOverlay.textures[0] = overlayRT;
        }


        /// <summary>
        /// Usage: block main thread with an empty for loop and generate a bunch of cubes around the player.
        /// </summary>
        void SimulateLevelLoad()
        {
            int numToPrint = 0;
            for (int p = 0; p < numLoopsTrigger; p++)
            {
                numToPrint++;
            }

            Debug.Log("Finished " + numToPrint + " Loops");
            Vector3 playerPos = mainCamera.transform.position;
            playerPos.y = 0.5f;
            // Generate a bunch of blocks, "blocking" the mainthread ;)
            for (int j = 0; j < numLevels; j++)
            {
                for (var i = 0; i < numObjectsPerLevel; i++)
                {
                    var angle = i * Mathf.PI * 2 / numObjectsPerLevel;
                    float stagger = (i % 2 == 0) ? 1.5f : 1.0f;
                    var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * cubeSpawnRadius * stagger;
                    pos.y = j * heightBetweenItems;
                    var newInst = Instantiate(prefabForLevelLoadSim, pos + playerPos, Quaternion.identity);
                    var newObjTransform = newInst.transform;
                    newObjTransform.LookAt(playerPos);
                    Vector3 newAngle = newObjTransform.rotation.eulerAngles;
                    newAngle.x = 0.0f;
                    newObjTransform.rotation = Quaternion.Euler(newAngle);
                    spawnedCubes.Add(newInst);
                }
            }
        }


        /// <summary>
        /// Usage: destroy all created cubes and garbage collect.
        /// </summary>
        void ClearObjects()
        {
            for (int i = 0; i < spawnedCubes.Count; i++)
            {
                DestroyImmediate(spawnedCubes[i]);
            }

            spawnedCubes.Clear();
            GC.Collect();
        }

        #endregion

        #region Debug UI Handlers

        /// <summary>
        /// Usage: radio button handler.
        /// </summary>
        public void RadioPressed(string radioLabel, string group, Toggle t)
        {
            if (string.Compare(radioLabel, ovrOverlayID) == 0)
            {
                ActivateOVROverlay();
            }
            else if (string.Compare(radioLabel, applicationID) == 0)
            {
                ActivateWorldGeo();
            }
            else if (string.Compare(radioLabel, noneID) == 0)
            {
                ActivateNone();
            }
        }

        #endregion
    }
}
