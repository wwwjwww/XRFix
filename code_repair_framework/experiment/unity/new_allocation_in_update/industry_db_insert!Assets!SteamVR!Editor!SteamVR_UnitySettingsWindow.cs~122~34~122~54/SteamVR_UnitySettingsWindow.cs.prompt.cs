//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Prompt developers to use settings most compatible with SteamVR.
//
//=============================================================================

using UnityEngine;
using UnityEditor;
using System.IO;

namespace Valve.VR
{
    [InitializeOnLoad]
    public class SteamVR_UnitySettingsWindow : EditorWindow
    {
        const bool forceShow = false; // Set to true to get the dialog to show back up in the case you clicked Ignore All.

        const string ignore = "ignore.";
        const string useRecommended = "Use recommended ({0})";
        const string currentValue = " (current = {0})";

        const string buildTarget = "Build Target";
        const string showUnitySplashScreen = "Show Unity Splashscreen";
        const string defaultIsFullScreen = "Default is Fullscreen";
        const string defaultScreenSize = "Default Screen Size";
        const string runInBackground = "Run In Background";
        const string displayResolutionDialog = "Display Resolution Dialog";
        const string resizableWindow = "Resizable Window";
        const string fullscreenMode = "D3D11 Fullscreen Mode";
        const string visibleInBackground = "Visible In Background";
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        const string renderingPath = "Rendering Path";
#endif
        const string colorSpace = "Color Space";
        const string gpuSkinning = "GPU Skinning";
#if false // skyboxes are currently broken
	const string singlePassStereoRendering = "Single-Pass Stereo Rendering";
#endif

        const BuildTarget recommended_BuildTarget = BuildTarget.StandaloneWindows64;
        const bool recommended_ShowUnitySplashScreen = false;
        const bool recommended_DefaultIsFullScreen = false;
        const int recommended_DefaultScreenWidth = 1024;
        const int recommended_DefaultScreenHeight = 768;
        const bool recommended_RunInBackground = true;
#if !UNITY_2019_1_OR_NEWER
        const ResolutionDialogSetting recommended_DisplayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
#endif
        const bool recommended_ResizableWindow = true;
        const D3D11FullscreenMode recommended_FullscreenMode = D3D11FullscreenMode.FullscreenWindow;
        const bool recommended_VisibleInBackground = true;
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        const RenderingPath recommended_RenderPath = RenderingPath.Forward;
#endif
        const ColorSpace recommended_ColorSpace = ColorSpace.Linear;
        const bool recommended_GpuSkinning = true;
#if false
	const bool recommended_SinglePassStereoRendering = true;
#endif

#if UNITY_2018_1_OR_NEWER
    const FullScreenMode recommended_FullScreenMode = FullScreenMode.FullScreenWindow;
#endif
        static SteamVR_UnitySettingsWindow window;

        static SteamVR_UnitySettingsWindow()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
                // BUG: Using New() allocation in Update() method.
                // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                //             bool show =
                //                 (!EditorPrefs.HasKey(ignore + buildTarget) &&
                //                     EditorUserBuildSettings.activeBuildTarget != recommended_BuildTarget) ||
                //                 (!EditorPrefs.HasKey(ignore + showUnitySplashScreen) &&
                // #if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                //                 PlayerSettings.showUnitySplashScreen != recommended_ShowUnitySplashScreen) ||
                // #else
                // 				PlayerSettings.SplashScreen.show != recommended_ShowUnitySplashScreen) ||
                // #endif
                // 
                // #if UNITY_2018_1_OR_NEWER
                //             (!EditorPrefs.HasKey(ignore + defaultIsFullScreen) &&
                //                 PlayerSettings.fullScreenMode != recommended_FullScreenMode) ||
                // #else
                //             (!EditorPrefs.HasKey(ignore + defaultIsFullScreen) &&
                //                     PlayerSettings.defaultIsFullScreen != recommended_DefaultIsFullScreen) ||
                //                 (!EditorPrefs.HasKey(ignore + fullscreenMode) &&
                //                     PlayerSettings.d3d11FullscreenMode != recommended_FullscreenMode) ||
                // #endif
                //             (!EditorPrefs.HasKey(ignore + defaultScreenSize) &&
                //                     (PlayerSettings.defaultScreenWidth != recommended_DefaultScreenWidth ||
                //                     PlayerSettings.defaultScreenHeight != recommended_DefaultScreenHeight)) ||
                //                 (!EditorPrefs.HasKey(ignore + runInBackground) &&
                //                     PlayerSettings.runInBackground != recommended_RunInBackground) ||
                // #if !UNITY_2019_1_OR_NEWER
                //                 (!EditorPrefs.HasKey(ignore + displayResolutionDialog) &&
                //                     PlayerSettings.displayResolutionDialog != recommended_DisplayResolutionDialog) ||
                // #endif
                //                 (!EditorPrefs.HasKey(ignore + resizableWindow) &&
                //                     PlayerSettings.resizableWindow != recommended_ResizableWindow) ||
                //                 (!EditorPrefs.HasKey(ignore + visibleInBackground) &&
                //                     PlayerSettings.visibleInBackground != recommended_VisibleInBackground) ||
                // #if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                //             (!EditorPrefs.HasKey(ignore + renderingPath) &&
                //                     PlayerSettings.renderingPath != recommended_RenderPath) ||
                // #endif
                //             (!EditorPrefs.HasKey(ignore + colorSpace) &&
                //                     PlayerSettings.colorSpace != recommended_ColorSpace) ||
                //                 (!EditorPrefs.HasKey(ignore + gpuSkinning) &&
                //                     PlayerSettings.gpuSkinning != recommended_GpuSkinning) ||
                // #if false
                // 			(!EditorPrefs.HasKey(ignore + singlePassStereoRendering) &&
                // 				PlayerSettings.singlePassStereoRendering != recommended_SinglePassStereoRendering) ||
                // #endif
                //             forceShow;
                // 
                //             if (show)
                //             {
                //                 window = GetWindow<SteamVR_UnitySettingsWindow>(true);
                //                 window.minSize = new Vector2(320, 440);
                //                 //window.title = "SteamVR";
                //             }
                // 
                //             string[] dlls = new string[]
                //             {
                //             "Plugins/x86/openvr_api.dll",
                //             "Plugins/x86_64/openvr_api.dll"
                //             };
                // 
                //             foreach (string path in dlls)
                //             {
                //                 if (!File.Exists(Application.dataPath + "/" + path))
                //                     continue;
                // 
                //                 if (AssetDatabase.DeleteAsset("Assets/" + path))
                //                     Debug.Log("<b>[SteamVR Setup]</b> Deleting " + path);
                //                 else
                //                 {
                //                     Debug.Log("<b>[SteamVR Setup]</b> " + path + " in use; cannot delete.  Please restart Unity to complete upgrade.");
                //                 }
                //             }
                // 
                //             EditorApplication.update -= Update;
                //         }

                // FIXED VERSION:
