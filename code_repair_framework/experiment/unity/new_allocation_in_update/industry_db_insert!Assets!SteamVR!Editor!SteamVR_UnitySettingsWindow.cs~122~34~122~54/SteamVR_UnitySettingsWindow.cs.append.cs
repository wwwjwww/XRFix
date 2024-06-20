
        Vector2 scrollPosition;

        string GetResourcePath()
        {
            var ms = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(ms);
            path = Path.GetDirectoryName(path);
            return path.Substring(0, path.Length - "Editor".Length) + "Textures/";
        }

        public void OnGUI()
        {
            var resourcePath = GetResourcePath();
            var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(resourcePath + "logo.png");
            var rect = GUILayoutUtility.GetRect(position.width, 150, GUI.skin.box);
            if (logo)
                GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);

            EditorGUILayout.HelpBox("Recommended project settings for SteamVR:", MessageType.Warning);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            int numItems = 0;

            if (!EditorPrefs.HasKey(ignore + buildTarget) &&
                EditorUserBuildSettings.activeBuildTarget != recommended_BuildTarget)
            {
                ++numItems;

                GUILayout.Label(buildTarget + string.Format(currentValue, EditorUserBuildSettings.activeBuildTarget));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_BuildTarget)))
                {
#if (UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                    EditorUserBuildSettings.SwitchActiveBuildTarget(recommended_BuildTarget);
#else
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, recommended_BuildTarget);
#endif
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + buildTarget, true);
                }

                GUILayout.EndHorizontal();
            }

#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
            if (!EditorPrefs.HasKey(ignore + showUnitySplashScreen) &&
                PlayerSettings.showUnitySplashScreen != recommended_ShowUnitySplashScreen)
            {
                ++numItems;

                GUILayout.Label(showUnitySplashScreen + string.Format(currentValue, PlayerSettings.showUnitySplashScreen));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_ShowUnitySplashScreen)))
                {
                    PlayerSettings.showUnitySplashScreen = recommended_ShowUnitySplashScreen;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + showUnitySplashScreen, true);
                }

                GUILayout.EndHorizontal();
            }
#else
		if (!EditorPrefs.HasKey(ignore + showUnitySplashScreen) &&
			PlayerSettings.SplashScreen.show != recommended_ShowUnitySplashScreen)
		{
			++numItems;

			GUILayout.Label(showUnitySplashScreen + string.Format(currentValue, PlayerSettings.SplashScreen.show));

			GUILayout.BeginHorizontal();

			if (GUILayout.Button(string.Format(useRecommended, recommended_ShowUnitySplashScreen)))
			{
				PlayerSettings.SplashScreen.show = recommended_ShowUnitySplashScreen;
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Ignore"))
			{
				EditorPrefs.SetBool(ignore + showUnitySplashScreen, true);
			}

			GUILayout.EndHorizontal();
		}
#endif

#if UNITY_2018_1_OR_NEWER
#else
            if (!EditorPrefs.HasKey(ignore + defaultIsFullScreen) &&
                PlayerSettings.defaultIsFullScreen != recommended_DefaultIsFullScreen)
            {
                ++numItems;

                GUILayout.Label(defaultIsFullScreen + string.Format(currentValue, PlayerSettings.defaultIsFullScreen));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_DefaultIsFullScreen)))
                {
                    PlayerSettings.defaultIsFullScreen = recommended_DefaultIsFullScreen;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + defaultIsFullScreen, true);
                }

                GUILayout.EndHorizontal();
            }
#endif

            if (!EditorPrefs.HasKey(ignore + defaultScreenSize) &&
                (PlayerSettings.defaultScreenWidth != recommended_DefaultScreenWidth ||
                PlayerSettings.defaultScreenHeight != recommended_DefaultScreenHeight))
            {
                ++numItems;

                GUILayout.Label(defaultScreenSize + string.Format(" ({0}x{1})", PlayerSettings.defaultScreenWidth, PlayerSettings.defaultScreenHeight));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format("Use recommended ({0}x{1})", recommended_DefaultScreenWidth, recommended_DefaultScreenHeight)))
                {
                    PlayerSettings.defaultScreenWidth = recommended_DefaultScreenWidth;
                    PlayerSettings.defaultScreenHeight = recommended_DefaultScreenHeight;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + defaultScreenSize, true);
                }

                GUILayout.EndHorizontal();
            }

            if (!EditorPrefs.HasKey(ignore + runInBackground) &&
                PlayerSettings.runInBackground != recommended_RunInBackground)
            {
                ++numItems;

                GUILayout.Label(runInBackground + string.Format(currentValue, PlayerSettings.runInBackground));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_RunInBackground)))
                {
                    PlayerSettings.runInBackground = recommended_RunInBackground;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + runInBackground, true);
                }

                GUILayout.EndHorizontal();
            }

#if !UNITY_2019_1_OR_NEWER
            if (!EditorPrefs.HasKey(ignore + displayResolutionDialog) &&
                PlayerSettings.displayResolutionDialog != recommended_DisplayResolutionDialog)
            {
                ++numItems;

                GUILayout.Label(displayResolutionDialog + string.Format(currentValue, PlayerSettings.displayResolutionDialog));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_DisplayResolutionDialog)))
                {
                    PlayerSettings.displayResolutionDialog = recommended_DisplayResolutionDialog;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + displayResolutionDialog, true);
                }

                GUILayout.EndHorizontal();
            }
#endif

            if (!EditorPrefs.HasKey(ignore + resizableWindow) &&
                PlayerSettings.resizableWindow != recommended_ResizableWindow)
            {
                ++numItems;

                GUILayout.Label(resizableWindow + string.Format(currentValue, PlayerSettings.resizableWindow));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_ResizableWindow)))
                {
                    PlayerSettings.resizableWindow = recommended_ResizableWindow;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + resizableWindow, true);
                }

                GUILayout.EndHorizontal();
            }
#if UNITY_2018_1_OR_NEWER
        if (!EditorPrefs.HasKey(ignore + defaultIsFullScreen) &&
			PlayerSettings.fullScreenMode != recommended_FullScreenMode)
#else
            if (!EditorPrefs.HasKey(ignore + fullscreenMode) &&
                PlayerSettings.d3d11FullscreenMode != recommended_FullscreenMode)
#endif
            {
                ++numItems;

#if UNITY_2018_1_OR_NEWER
            GUILayout.Label(fullscreenMode + string.Format(currentValue, PlayerSettings.fullScreenMode));
#else
                GUILayout.Label(fullscreenMode + string.Format(currentValue, PlayerSettings.d3d11FullscreenMode));
#endif

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_FullscreenMode)))
                {
#if UNITY_2018_1_OR_NEWER
                PlayerSettings.fullScreenMode = recommended_FullScreenMode;
#else
                    PlayerSettings.d3d11FullscreenMode = recommended_FullscreenMode;
#endif
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + fullscreenMode, true);
                }

                GUILayout.EndHorizontal();
            }

            if (!EditorPrefs.HasKey(ignore + visibleInBackground) &&
                PlayerSettings.visibleInBackground != recommended_VisibleInBackground)
            {
                ++numItems;

                GUILayout.Label(visibleInBackground + string.Format(currentValue, PlayerSettings.visibleInBackground));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_VisibleInBackground)))
                {
                    PlayerSettings.visibleInBackground = recommended_VisibleInBackground;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + visibleInBackground, true);
                }

                GUILayout.EndHorizontal();
            }
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
            if (!EditorPrefs.HasKey(ignore + renderingPath) &&
                PlayerSettings.renderingPath != recommended_RenderPath)
            {
                ++numItems;

                GUILayout.Label(renderingPath + string.Format(currentValue, PlayerSettings.renderingPath));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_RenderPath) + " - required for MSAA"))
                {
                    PlayerSettings.renderingPath = recommended_RenderPath;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + renderingPath, true);
                }

                GUILayout.EndHorizontal();
            }
#endif
            if (!EditorPrefs.HasKey(ignore + colorSpace) &&
                PlayerSettings.colorSpace != recommended_ColorSpace)
            {
                ++numItems;

                GUILayout.Label(colorSpace + string.Format(currentValue, PlayerSettings.colorSpace));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_ColorSpace) + " - requires reloading scene"))
                {
                    PlayerSettings.colorSpace = recommended_ColorSpace;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + colorSpace, true);
                }

                GUILayout.EndHorizontal();
            }

            if (!EditorPrefs.HasKey(ignore + gpuSkinning) &&
                PlayerSettings.gpuSkinning != recommended_GpuSkinning)
            {
                ++numItems;

                GUILayout.Label(gpuSkinning + string.Format(currentValue, PlayerSettings.gpuSkinning));

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(string.Format(useRecommended, recommended_GpuSkinning)))
                {
                    PlayerSettings.gpuSkinning = recommended_GpuSkinning;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ignore"))
                {
                    EditorPrefs.SetBool(ignore + gpuSkinning, true);
                }

                GUILayout.EndHorizontal();
            }

#if false
		if (!EditorPrefs.HasKey(ignore + singlePassStereoRendering) &&
			PlayerSettings.singlePassStereoRendering != recommended_SinglePassStereoRendering)
		{
			++numItems;

			GUILayout.Label(singlePassStereoRendering + string.Format(currentValue, PlayerSettings.singlePassStereoRendering));

			GUILayout.BeginHorizontal();

			if (GUILayout.Button(string.Format(useRecommended, recommended_SinglePassStereoRendering)))
			{
				PlayerSettings.singlePassStereoRendering = recommended_SinglePassStereoRendering;
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Ignore"))
			{
				EditorPrefs.SetBool(ignore + singlePassStereoRendering, true);
			}

			GUILayout.EndHorizontal();
		}
#endif

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear All Ignores"))
            {
                EditorPrefs.DeleteKey(ignore + buildTarget);
                EditorPrefs.DeleteKey(ignore + showUnitySplashScreen);
                EditorPrefs.DeleteKey(ignore + defaultIsFullScreen);
                EditorPrefs.DeleteKey(ignore + defaultScreenSize);
                EditorPrefs.DeleteKey(ignore + runInBackground);
                EditorPrefs.DeleteKey(ignore + displayResolutionDialog);
                EditorPrefs.DeleteKey(ignore + resizableWindow);
                EditorPrefs.DeleteKey(ignore + fullscreenMode);
                EditorPrefs.DeleteKey(ignore + visibleInBackground);
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                EditorPrefs.DeleteKey(ignore + renderingPath);
#endif
                EditorPrefs.DeleteKey(ignore + colorSpace);
                EditorPrefs.DeleteKey(ignore + gpuSkinning);
#if false
			EditorPrefs.DeleteKey(ignore + singlePassStereoRendering);
#endif
            }

            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if (numItems > 0)
            {
                if (GUILayout.Button("Accept All"))
                {
                    // Only set those that have not been explicitly ignored.
                    if (!EditorPrefs.HasKey(ignore + buildTarget))
#if (UNITY_5_5 || UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                        EditorUserBuildSettings.SwitchActiveBuildTarget(recommended_BuildTarget);
#else
					EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, recommended_BuildTarget);
#endif
                    if (!EditorPrefs.HasKey(ignore + showUnitySplashScreen))
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                        PlayerSettings.showUnitySplashScreen = recommended_ShowUnitySplashScreen;
#else
					PlayerSettings.SplashScreen.show = recommended_ShowUnitySplashScreen;
#endif

#if UNITY_2018_1_OR_NEWER
                if (!EditorPrefs.HasKey(ignore + defaultIsFullScreen))
                    PlayerSettings.fullScreenMode = recommended_FullScreenMode;
#else
                    if (!EditorPrefs.HasKey(ignore + defaultIsFullScreen))
                        PlayerSettings.defaultIsFullScreen = recommended_DefaultIsFullScreen;
                    if (!EditorPrefs.HasKey(ignore + fullscreenMode))
                        PlayerSettings.d3d11FullscreenMode = recommended_FullscreenMode;
#endif
                    if (!EditorPrefs.HasKey(ignore + defaultScreenSize))
                    {
                        PlayerSettings.defaultScreenWidth = recommended_DefaultScreenWidth;
                        PlayerSettings.defaultScreenHeight = recommended_DefaultScreenHeight;
                    }
                    if (!EditorPrefs.HasKey(ignore + runInBackground))
                        PlayerSettings.runInBackground = recommended_RunInBackground;
#if !UNITY_2019_1_OR_NEWER
                    if (!EditorPrefs.HasKey(ignore + displayResolutionDialog))
                        PlayerSettings.displayResolutionDialog = recommended_DisplayResolutionDialog;
#endif
                    if (!EditorPrefs.HasKey(ignore + resizableWindow))
                        PlayerSettings.resizableWindow = recommended_ResizableWindow;
                    if (!EditorPrefs.HasKey(ignore + visibleInBackground))
                        PlayerSettings.visibleInBackground = recommended_VisibleInBackground;
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                    if (!EditorPrefs.HasKey(ignore + renderingPath))
                        PlayerSettings.renderingPath = recommended_RenderPath;
#endif
                    if (!EditorPrefs.HasKey(ignore + colorSpace))
                        PlayerSettings.colorSpace = recommended_ColorSpace;
                    if (!EditorPrefs.HasKey(ignore + gpuSkinning))
                        PlayerSettings.gpuSkinning = recommended_GpuSkinning;
#if false
				if (!EditorPrefs.HasKey(ignore + singlePassStereoRendering))
					PlayerSettings.singlePassStereoRendering = recommended_SinglePassStereoRendering;
#endif

                    EditorUtility.DisplayDialog("Accept All", "You made the right choice!", "Ok");

                    Close();
                }

                if (GUILayout.Button("Ignore All"))
                {
                    if (EditorUtility.DisplayDialog("Ignore All", "Are you sure?", "Yes, Ignore All", "Cancel"))
                    {
                        // Only ignore those that do not currently match our recommended settings.
                        if (EditorUserBuildSettings.activeBuildTarget != recommended_BuildTarget)
                            EditorPrefs.SetBool(ignore + buildTarget, true);
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                        if (PlayerSettings.showUnitySplashScreen != recommended_ShowUnitySplashScreen)
#else
					if (PlayerSettings.SplashScreen.show != recommended_ShowUnitySplashScreen)
#endif
                            EditorPrefs.SetBool(ignore + showUnitySplashScreen, true);

#if UNITY_2018_1_OR_NEWER
                    if (PlayerSettings.fullScreenMode != recommended_FullScreenMode)
                    {
                        EditorPrefs.SetBool(ignore + defaultIsFullScreen, true);
                        EditorPrefs.SetBool(ignore + fullscreenMode, true);
                    }
#else
                        if (PlayerSettings.defaultIsFullScreen != recommended_DefaultIsFullScreen)
                            EditorPrefs.SetBool(ignore + defaultIsFullScreen, true);
                        if (PlayerSettings.d3d11FullscreenMode != recommended_FullscreenMode)
                            EditorPrefs.SetBool(ignore + fullscreenMode, true);
#endif
                        if (PlayerSettings.defaultScreenWidth != recommended_DefaultScreenWidth ||
                            PlayerSettings.defaultScreenHeight != recommended_DefaultScreenHeight)
                            EditorPrefs.SetBool(ignore + defaultScreenSize, true);
                        if (PlayerSettings.runInBackground != recommended_RunInBackground)
                            EditorPrefs.SetBool(ignore + runInBackground, true);
#if !UNITY_2019_1_OR_NEWER
                        if (PlayerSettings.displayResolutionDialog != recommended_DisplayResolutionDialog)
                            EditorPrefs.SetBool(ignore + displayResolutionDialog, true);
#endif
                        if (PlayerSettings.resizableWindow != recommended_ResizableWindow)
                            EditorPrefs.SetBool(ignore + resizableWindow, true);
                        if (PlayerSettings.visibleInBackground != recommended_VisibleInBackground)
                            EditorPrefs.SetBool(ignore + visibleInBackground, true);
#if (UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                        if (PlayerSettings.renderingPath != recommended_RenderPath)
                            EditorPrefs.SetBool(ignore + renderingPath, true);
#endif
                        if (PlayerSettings.colorSpace != recommended_ColorSpace)
                            EditorPrefs.SetBool(ignore + colorSpace, true);
                        if (PlayerSettings.gpuSkinning != recommended_GpuSkinning)
                            EditorPrefs.SetBool(ignore + gpuSkinning, true);
#if false
					if (PlayerSettings.singlePassStereoRendering != recommended_SinglePassStereoRendering)
						EditorPrefs.SetBool(ignore + singlePassStereoRendering, true);
#endif

                        Close();
                    }
                }
            }
            else if (GUILayout.Button("Close"))
            {
                Close();
            }

            GUILayout.EndHorizontal();
        }
    }
}