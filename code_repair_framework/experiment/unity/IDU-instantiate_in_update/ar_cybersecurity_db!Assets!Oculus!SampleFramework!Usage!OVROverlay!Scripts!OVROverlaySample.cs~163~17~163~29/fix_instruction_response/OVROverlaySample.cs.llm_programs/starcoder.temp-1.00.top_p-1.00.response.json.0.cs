/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace OculusSampleFramework
{



    public enum EUiDisplayType
    {
        EUDT_WorldGeoQuad,
        EUDT_OverlayQuad,
        EUDT_None,
        EUDT_MaxDislayTypes
    }











    public class OVROverlaySample : MonoBehaviour
    {
        bool inMenu;




        const string ovrOverlayID = "OVROverlayID";

        const string applicationID = "ApplicationID";
        const string noneID = "NoneID";




        Toggle applicationRadioButton;

        Toggle noneRadioButton;

        [Header("App vs Compositor Comparison Settings")]
        public GameObject mainCamera;




        public GameObject uiCamera;




        public GameObject uiGeoParent;

        public GameObject worldspaceGeoParent;




        public OVROverlay cameraRenderOverlay;




        public OVROverlay renderingLabelOverlay;




        public Texture applicationLabelTexture;

        public Texture compositorLabelTexture;




        [Header("Level Loading Sim Settings")]
        public GameObject prefabForLevelLoadSim;

        public OVROverlay cubemapOverlay;
        public OVROverlay loadingTextQuadOverlay;
        public float distanceFromCamToLoadText;
        public float cubeSpawnRadius;
        public float heightBetweenItems;
        public int numObjectsPerLevel;
        public int numLevels;
        public int numLoopsTrigger;
        List<GameObject> spawnedCubes = new List<GameObject>();

        #region MonoBehaviour handler

        private void Start()
        {
            DebugUIBuilder.instance.AddLabel("OVROverlay Sample");
            DebugUIBuilder.instance.AddDivider();
            DebugUIBuilder.instance.AddLabel("Level Loading Example");
            DebugUIBuilder.instance.AddButton("Simulate Level Load", TriggerLoad);
            DebugUIBuilder.instance.AddButton("Destroy Cubes", TriggerUnload);
            DebugUIBuilder.instance.AddDivider();
            DebugUIBuilder.instance.AddLabel("OVROverlay vs. Application Render Comparison");
            DebugUIBuilder.instance
               .AddRadio("OVROverlay", "group", delegate(Toggle t) { RadioPressed(ovrOverlayID, "group", t); })
               .GetComponentInChildren<Toggle>();
            var applicationRadio = DebugUIBuilder.instance
               .AddRadio("Application", "group", delegate(Toggle t) { RadioPressed(applicationID, "group", t); })
               .GetComponentInChildren<Toggle>();
            var noneRadio = DebugUIBuilder.instance
               .AddRadio("None", "group", delegate(Toggle t) { RadioPressed(noneID, "group", t); })
               .GetComponentInChildren<Toggle>();

            DebugUIBuilder.instance.Show();

            CameraAndRenderTargetSetup();
            cameraRenderOverlay.enabled = true;
            cameraRenderOverlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
        }


// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//        void Update()
//        {
//
//            if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
//            {
//                if (inMenu) DebugUIBuilder.instance.Hide();
//                else DebugUIBuilder.instance.Show();
//                inMenu = !inMenu;
//            }
//
//
//            if (Input.GetKeyDown(KeyCode.A))
//            {
//                TriggerLoad();
//            }
//        }


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







        private void TriggerLoad()
        {
            ClearObjects();
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
            noneRadio.isOn = true;
            yield return new WaitForSeconds(0.1f);
            SimulateLevelLoad();
            cubemapOverlay.enabled = false;
            loadingTextQuadOverlay.enabled = false;
            yield return null;
        }





        private void TriggerUnload()
        {
            ClearObjects();
            applicationRadio.isOn = true;
        }




        private void CameraAndRenderTargetSetup()
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

            if (uiCamera.GetComponent<Camera>().targetTexture!= null)
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








        [Header("Object Pool Settings")]
        public GameObject prefabForLevelLoadSim;

        private Queue<GameObject> objectPool = new Queue<GameObject>();

        private void Awake()
        {
            // Pre-instantiate objects to avoid expensive instantiation in Update()
            for (int i = 0; i < numObjectsPerLevel * numLevels; i++)
            {
                var obj = Instantiate(prefabForLevelLoadSim);
                objectPool.Enqueue(obj);
                obj.SetActive(false);
            }
        }

        void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
            {
                if (inMenu) DebugUIBuilder.instance.Hide();
                else DebugUIBuilder.instance.Show();
                inMenu =!inMenu;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                TriggerLoad();
            }
        }

        private void SimulateLevelLoad()
        {
            int numToPrint = 0;
            for (int p = 0; p < numLoopsTrigger; p++)
            {
                numToPrint++;
            }

            Debug.Log("Finished " + numToPrint + " Loops");
            Vector3 playerPos = mainCamera.transform.position;
            playerPos.y = 0.5f;

            for (int j = 0; j < numLevels; j++)
            {
                for (var i = 0; i < numObjectsPerLevel; i++)
                {
                    var angle = i * Mathf.PI * 2 / numObjectsPerLevel;
                    float stagger = (i % 2 == 0)? 1.5f : 1.0f;
                    var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * cubeSpawnRadius * stagger;
                    pos.y = j * heightBetweenItems;
                    var newInst = TakeFromPool();
                    var newObjTransform = newInst.transform;
                    newObjTransform.position = pos + playerPos;
                    spawnedCubes.Add(newInst);
                }
            }
        }

        private void TakeFromPool()
        {
            if (objectPool.Count > 0)
            {
                var obj = objectPool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                var obj = Instantiate(prefabForLevelLoadSim);
                return obj;
            }
        }

        private void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            objectPool.Enqueue(obj);
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
