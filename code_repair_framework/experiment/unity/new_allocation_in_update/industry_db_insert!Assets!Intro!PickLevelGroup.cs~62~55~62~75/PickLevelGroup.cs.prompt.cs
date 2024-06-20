using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BaroqueUI;


namespace NanIndustryVR
{
    public class PickLevelGroup : MonoBehaviour
    {
        const string BASE_URL = "https://baroquesoftware.com/nan-industry/";

        public Transform groupsContent;
        public Button categoryButtonPrefab, extraButtonPrefab;
        public LevelPlatformsBand band;
        public GameObject canvasHelp;
        public Text helpText;

        float intensity1, intensity2, current_intensity_fraction;


        static Level.Store store;
        static string current_category = "", target_category = "";

        private void Start()
        {
            intensity1 = RenderSettings.ambientIntensity;
            intensity2 = RenderSettings.sun.intensity;
            RenderSettings.ambientIntensity = 0f;
            RenderSettings.sun.intensity = 0f;
            current_intensity_fraction = 0f;

            if (store == null)
            {
                LoadLocalStoreLevels();
                StartRefreshingLevels();
            }
            StoreUpdated();
        }

        private void Update()
        {
                        // BUG: Using New() allocation in Update() method.
                        // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                        //             if (current_category == target_category)
                        //             {
                        //                 if (current_intensity_fraction == 1f)
                        //                     return;
                        // 
                        //                 current_intensity_fraction += Time.deltaTime * 1.4f;
                        //                 if (current_intensity_fraction >= 1f)
                        //                 {
                        //                     current_intensity_fraction = 1f;
                        // 
                        //                     if (current_category == Level.LevelFile.CATEGORY_CUSTOM)
                        //                     {
                        //                         RenderSettings.ambientIntensity = intensity1 * 1.5f;
                        //                         var old_mode = RenderSettings.ambientMode;
                        //                         RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                        //                         RenderSettings.ambientLight = new Color(1f, 1f, 1f);
                        // 
                        //                         band.UpdateScreenshots();
                        // 
                        //                         RenderSettings.ambientMode = old_mode;
                        //                     }
                        //                 }
                        //             }
                        //             else
                        //             {
                        //                 current_intensity_fraction -= Time.deltaTime * 2f;
                        //                 if (current_intensity_fraction <= 0f)
                        //                 {
                        //                     current_intensity_fraction = 0f;
                        //                     current_category = target_category;
                        //                     StoreUpdated();
                        //                 }
                        //             }
                        // 
                        //             RenderSettings.ambientIntensity = intensity1 * current_intensity_fraction;
                        //             RenderSettings.sun.intensity = intensity2 * current_intensity_fraction;
                        //         }

                        // FIXED VERSION:
