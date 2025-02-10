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
 * https:
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
        public int numLoopsTrigger = 500000000;
        List<GameObject> spawnedCubes = new List<GameObject>();

        #region MonoBehaviour handler

        void Start()
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
            applicationRadioButton = DebugUIBuilder.instance
                .AddRadio("Application", "group", delegate(Toggle t) { RadioPressed(applicationID, "group", t); })
                .GetComponentInChildren<Toggle>();
            noneRadioButton = DebugUIBuilder.instance
                .AddRadio("None", "group", delegate(Toggle t) { RadioPressed(noneID, "group", t); })
                .GetComponentInChildren<Toggle>();

            DebugUIBuilder.instance.Show();

            
            CameraAndRenderTargetSetup();
            cameraRenderOverlay.enabled = true;
            cameraRenderOverlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
            spawnedCubes.Capacity = numObjectsPerLevel * numLevels;
        }

