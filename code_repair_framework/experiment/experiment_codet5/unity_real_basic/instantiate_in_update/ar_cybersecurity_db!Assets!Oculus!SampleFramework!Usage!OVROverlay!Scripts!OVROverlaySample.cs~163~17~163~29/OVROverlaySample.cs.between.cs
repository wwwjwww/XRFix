
        #endregion

        #region Private Functions




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




        void ActivateNone()
        {
            worldspaceGeoParent.SetActive(false);
            uiCamera.SetActive(false);
            cameraRenderOverlay.enabled = false;
            uiGeoParent.SetActive(false);
            renderingLabelOverlay.enabled = false;
            Debug.Log("Switched to ActivateNone");
        }







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





        void TriggerUnload()
        {
            ClearObjects();
            applicationRadioButton.isOn = true;
        }




        void CameraAndRenderTargetSetup()
        {
            float overlayWidth = cameraRenderOverlay.transform.localScale.x;
            float overlayHeight = cameraRenderOverlay.transform.localScale.y;
            float overlayRadius = cameraRenderOverlay.transform.localScale.z;

#if UNITY_ANDROID

            float hmdPanelResWidth = 2560;
            float hmdPanelResHeight = 1440;
#else

            float hmdPanelResWidth = 2160;
            float hmdPanelResHeight = 1200;
#endif

            float singleEyeScreenPhysicalResX = hmdPanelResWidth * 0.5f;
            float singleEyeScreenPhysicalResY = hmdPanelResHeight;





            float halfFovY = mainCamera.GetComponent<Camera>().fieldOfView / 2;
            float screenSizeYInWorld = 2 * overlayRadius * Mathf.Tan(Mathf.Deg2Rad * halfFovY);
            float pixelDensityYPerWorldUnit = singleEyeScreenPhysicalResY / screenSizeYInWorld;
            float renderTargetHeight = pixelDensityYPerWorldUnit * overlayWidth;


            float renderTargetWidth = 0.0f;




            float screenSizeXInWorld = screenSizeYInWorld * mainCamera.GetComponent<Camera>().aspect;
            float pixelDensityXPerWorldUnit = singleEyeScreenPhysicalResX / screenSizeXInWorld;
            renderTargetWidth = pixelDensityXPerWorldUnit * overlayWidth;


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





