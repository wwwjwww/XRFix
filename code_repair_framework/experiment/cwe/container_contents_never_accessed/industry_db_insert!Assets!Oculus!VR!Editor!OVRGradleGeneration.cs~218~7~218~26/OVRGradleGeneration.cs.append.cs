
	public void PatchAndroidManifest(string path)
	{
		string manifestFolder = Path.Combine(path, "src/main");
		string file = manifestFolder + "/AndroidManifest.xml";

		bool patchedSecurityConfig = false;
		// If Enable NSC Config, copy XML file into gradle project
		OVRProjectConfig projectConfig = OVRProjectConfig.GetProjectConfig();
		if (projectConfig != null)
		{
			if (projectConfig.enableNSCConfig)
			{
				// If no custom xml security path is specified, look for the default location in the integrations package.
				string securityConfigFile = projectConfig.securityXmlPath;
				if (string.IsNullOrEmpty(securityConfigFile))
				{
					securityConfigFile = GetOculusProjectNetworkSecConfigPath();
				}
				else
				{
					Uri configUri = new Uri(Path.GetFullPath(securityConfigFile));
					Uri projectUri = new Uri(Application.dataPath);
					Uri relativeUri = projectUri.MakeRelativeUri(configUri);
					securityConfigFile = relativeUri.ToString();
				}

				string xmlDirectory = Path.Combine(path, "src/main/res/xml");
				try
				{
					if (!Directory.Exists(xmlDirectory))
					{
						Directory.CreateDirectory(xmlDirectory);
					}
					File.Copy(securityConfigFile, Path.Combine(xmlDirectory, "network_sec_config.xml"), true);
					patchedSecurityConfig = true;
				}
				catch (Exception e)
				{
					UnityEngine.Debug.LogError(e.Message);
				}
			}
		}

		OVRManifestPreprocessor.PatchAndroidManifest(file, enableSecurity: patchedSecurityConfig);
	}

	private static string GetOculusProjectNetworkSecConfigPath()
	{
		var so = ScriptableObject.CreateInstance(typeof(OVRPluginInfo));
		var script = MonoScript.FromScriptableObject(so);
		string assetPath = AssetDatabase.GetAssetPath(script);
		string editorDir = Directory.GetParent(assetPath).FullName;
		string configAssetPath = Path.GetFullPath(Path.Combine(editorDir, "network_sec_config.xml"));
		Uri configUri = new Uri(configAssetPath);
		Uri projectUri = new Uri(Application.dataPath);
		Uri relativeUri = projectUri.MakeRelativeUri(configUri);

		return relativeUri.ToString();
	}

	public void OnPostprocessBuild(BuildReport report)
	{
#if UNITY_ANDROID
		if(autoIncrementVersion)
		{
			if((report.summary.options & BuildOptions.Development) == 0)
			{
				PlayerSettings.Android.bundleVersionCode++;
				UnityEngine.Debug.Log("Incrementing version code to " + PlayerSettings.Android.bundleVersionCode);
			}
		}

		bool isExporting = true;
		foreach (var step in report.steps)
		{
			if (step.name.Contains("Compile scripts")
				|| step.name.Contains("Building scenes")
				|| step.name.Contains("Writing asset files")
				|| step.name.Contains("Preparing APK resources")
				|| step.name.Contains("Creating Android manifest")
				|| step.name.Contains("Processing plugins")
				|| step.name.Contains("Exporting project")
				|| step.name.Contains("Building Gradle project"))
			{
#if BUILDSESSION
				UnityEngine.Debug.LogFormat("build_step_" + step.name.ToLower().Replace(' ', '_') + ": {0}", step.duration.TotalSeconds.ToString());
#endif
				if(step.name.Contains("Building Gradle project"))
				{
					isExporting = false;
				}
			}
		}
#endif
		if (!report.summary.outputPath.Contains("OVRGradleTempExport"))
		{
#if BUILDSESSION
			UnityEngine.Debug.LogFormat("build_complete: {0}", (System.DateTime.Now - buildStartTime).TotalSeconds.ToString());
#endif
		}

#if UNITY_ANDROID
		if (!isExporting)
		{
			// Get the hosts path to Android SDK
			if (adbTool == null)
			{
				adbTool = new OVRADBTool(OVRConfig.Instance.GetAndroidSDKPath(false));
			}

			if (adbTool.isReady)
			{
				// Check to see if there are any ADB devices connected before continuing.
				List<string> devices = adbTool.GetDevices();
				if(devices.Count == 0)
				{
					return;
				}

				// Clear current logs on device
				Process adbClearProcess;
				adbClearProcess = adbTool.RunCommandAsync(new string[] { "logcat --clear" }, null);

				// Add a timeout if we cannot get a response from adb logcat --clear in time.
				Stopwatch timeout = new Stopwatch();
				timeout.Start();
				while (!adbClearProcess.WaitForExit(100))
				{
					if (timeout.ElapsedMilliseconds > 2000)
					{
						adbClearProcess.Kill();
						return;
					}
				}

				// Check if existing ADB process is still running, kill if needed
				if (adbProcess != null && !adbProcess.HasExited)
				{
					adbProcess.Kill();
				}

				// Begin thread to time upload and install
				var thread = new Thread(delegate ()
				{
					TimeDeploy();
				});
				thread.Start();
			}
		}
#endif
	}

#if UNITY_ANDROID
	public bool WaitForProcess;
	public bool TransferStarted;
	public DateTime UploadStart;
	public DateTime UploadEnd;
	public DateTime InstallEnd;

	public void TimeDeploy()
	{
		if (adbTool != null)
		{
			TransferStarted = false;
			DataReceivedEventHandler outputRecieved = new DataReceivedEventHandler(
				(s, e) =>
				{
					if (e.Data != null && e.Data.Length != 0 && !e.Data.Contains("\u001b"))
					{
						if (e.Data.Contains("free_cache"))
						{
							// Device recieved install command and is starting upload
							UploadStart = System.DateTime.Now;
							TransferStarted = true;
						}
						else if (e.Data.Contains("Running dexopt"))
						{
							// Upload has finished and Package Manager is starting install
							UploadEnd = System.DateTime.Now;
						}
						else if (e.Data.Contains("dex2oat took"))
						{
							// Package Manager finished install
							InstallEnd = System.DateTime.Now;
							WaitForProcess = false;
						}
						else if (e.Data.Contains("W PackageManager"))
						{
							// Warning from Package Manager is a failure in the install process
							WaitForProcess = false;
						}
					}
				}
			);

			WaitForProcess = true;
			adbProcess = adbTool.RunCommandAsync(new string[] { "logcat" }, outputRecieved);

			Stopwatch transferTimeout = new Stopwatch();
			transferTimeout.Start();
			while (adbProcess != null && !adbProcess.WaitForExit(100))
			{
				if (!WaitForProcess)
				{
					adbProcess.Kill();
				}

				if (!TransferStarted && transferTimeout.ElapsedMilliseconds > 5000)
				{
					adbProcess.Kill();
				}
			}
		}
	}
#endif
}
