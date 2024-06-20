//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Prompt developers to use settings most compatible with SteamVR.
//
//=============================================================================

using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class SteamVR_Settings : EditorWindow
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
	const ResolutionDialogSetting recommended_DisplayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
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

	static SteamVR_Settings window;

	static SteamVR_Settings()
	{
		EditorApplication.update += Update;
	}

	static void Update()
	{
			// BUG: Using New() allocation in Update() method.
			// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
			// 		bool show =
			// 			(!EditorPrefs.HasKey(ignore + buildTarget) &&
			// 				EditorUserBuildSettings.activeBuildTarget != recommended_BuildTarget) ||
			// 			(!EditorPrefs.HasKey(ignore + showUnitySplashScreen) &&
			// #if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
			// 				PlayerSettings.showUnitySplashScreen != recommended_ShowUnitySplashScreen) ||
			// #else
			// 				PlayerSettings.SplashScreen.show != recommended_ShowUnitySplashScreen) ||
			// #endif
			// 			(!EditorPrefs.HasKey(ignore + defaultIsFullScreen) &&
			// 				PlayerSettings.defaultIsFullScreen != recommended_DefaultIsFullScreen) ||
			// 			(!EditorPrefs.HasKey(ignore + defaultScreenSize) &&
			// 				(PlayerSettings.defaultScreenWidth != recommended_DefaultScreenWidth ||
			// 				PlayerSettings.defaultScreenHeight != recommended_DefaultScreenHeight)) ||
			// 			(!EditorPrefs.HasKey(ignore + runInBackground) &&
			// 				PlayerSettings.runInBackground != recommended_RunInBackground) ||
			// 			(!EditorPrefs.HasKey(ignore + displayResolutionDialog) &&
			// 				PlayerSettings.displayResolutionDialog != recommended_DisplayResolutionDialog) ||
			// 			(!EditorPrefs.HasKey(ignore + resizableWindow) &&
			// 				PlayerSettings.resizableWindow != recommended_ResizableWindow) ||
			// 			(!EditorPrefs.HasKey(ignore + fullscreenMode) &&
			// 				PlayerSettings.d3d11FullscreenMode != recommended_FullscreenMode) ||
			// 			(!EditorPrefs.HasKey(ignore + visibleInBackground) &&
			// 				PlayerSettings.visibleInBackground != recommended_VisibleInBackground) ||
			// #if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
			// 			(!EditorPrefs.HasKey(ignore + renderingPath) &&
			// 				PlayerSettings.renderingPath != recommended_RenderPath) ||
			// #endif
			// 			(!EditorPrefs.HasKey(ignore + colorSpace) &&
			// 				PlayerSettings.colorSpace != recommended_ColorSpace) ||
			// 			(!EditorPrefs.HasKey(ignore + gpuSkinning) &&
			// 				PlayerSettings.gpuSkinning != recommended_GpuSkinning) ||
			// #if false
			// 			(!EditorPrefs.HasKey(ignore + singlePassStereoRendering) &&
			// 				PlayerSettings.singlePassStereoRendering != recommended_SinglePassStereoRendering) ||
			// #endif
			// 			forceShow;
			// 
			// 		if (show)
			// 		{
			// 			window = GetWindow<SteamVR_Settings>(true);
			// 			window.minSize = new Vector2(320, 440);
			// 			//window.title = "SteamVR";
			// 		}
			// 
			// 		if (SteamVR_Preferences.AutoEnableVR)
			// 		{
			// 			// Switch to native OpenVR support.
			// 			var updated = false;
			// 
			// 			if (!PlayerSettings.virtualRealitySupported)
			// 			{
			// 				PlayerSettings.virtualRealitySupported = true;
			// 				updated = true;
			// 			}
			// 
			// #if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
			// 			var devices = UnityEditorInternal.VR.VREditor.GetVREnabledDevices(BuildTargetGroup.Standalone);
			// #else
			// 			var devices = UnityEditorInternal.VR.VREditor.GetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Standalone);
			// #endif
			// 			var hasOpenVR = false;
			// 			foreach (var device in devices)
			// 				if (device.ToLower() == "openvr")
			// 					hasOpenVR = true;
			// 
			// 
			// 			if (!hasOpenVR)
			// 			{
			// 				string[] newDevices;
			// 				if (updated)
			// 				{
			// 					newDevices = new string[] { "OpenVR" };
			// 				}
			// 				else
			// 				{
			// 					newDevices = new string[devices.Length + 1];
			// 					for (int i = 0; i < devices.Length; i++)
			// 						newDevices[i] = devices[i];
			// 					newDevices[devices.Length] = "OpenVR";
			// 					updated = true;
			// 				}
			// #if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
			// 				UnityEditorInternal.VR.VREditor.SetVREnabledDevices(BuildTargetGroup.Standalone, newDevices);
			// #else
			// 				UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.Standalone, newDevices);
			// #endif
			// 			}
			// 
			// 			if (updated)
			// 				Debug.Log("Switching to native OpenVR support.");
			// 		}
			// 
			// 		var dlls = new string[]
			// 		{
			// 			"Plugins/x86/openvr_api.dll",
			// 			"Plugins/x86_64/openvr_api.dll"
			// 		};
			// 
			// 		foreach (var path in dlls)
			// 		{
			// 			if (!File.Exists(Application.dataPath + "/" + path))
			// 				continue;
			// 
			// 			if (AssetDatabase.DeleteAsset("Assets/" + path))
			// 				Debug.Log("Deleting " + path);
			// 			else
			// 			{
			// 				Debug.Log(path + " in use; cannot delete.  Please restart Unity to complete upgrade.");
			// 			}
			// 		}
			// 
			// 		EditorApplication.update -= Update;
			// 	}

			// FIXED VERSION:
