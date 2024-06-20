//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Notify developers when a new version of the plugin is available.
//
//=============================================================================

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class SteamVR_Update : EditorWindow
{
	const string currentVersion = "1.2.2";
	const string versionUrl = "http://media.steampowered.com/apps/steamvr/unitypluginversion.txt";
	const string notesUrl = "http://media.steampowered.com/apps/steamvr/unityplugin-v{0}.txt";
	const string pluginUrl = "http://u3d.as/content/valve-corporation/steam-vr-plugin";
	const string doNotShowKey = "SteamVR.DoNotShow.v{0}";

	static bool gotVersion = false;
	static WWW wwwVersion, wwwNotes;
	static string version, notes;
	static SteamVR_Update window;

	static SteamVR_Update()
	{
		EditorApplication.update += Update;
	}

	static void Update()
	{
				// BUG: Using New() allocation in Update() method.
				// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
				// 		if (!gotVersion)
				// 		{
				// 			if (wwwVersion == null)
				// 				wwwVersion = new WWW(versionUrl);
				// 
				// 			if (!wwwVersion.isDone)
				// 				return;
				// 
				// 			if (UrlSuccess(wwwVersion))
				// 				version = wwwVersion.text;
				// 
				// 			wwwVersion = null;
				// 			gotVersion = true;
				// 
				// 			if (ShouldDisplay())
				// 			{
				// 				var url = string.Format(notesUrl, version);
				// 				wwwNotes = new WWW(url);
				// 
				// 				window = GetWindow<SteamVR_Update>(true);
				// 				window.minSize = new Vector2(320, 440);
				// 				//window.title = "SteamVR";
				// 			}
				// 		}
				// 
				// 		if (wwwNotes != null)
				// 		{
				// 			if (!wwwNotes.isDone)
				// 				return;
				// 
				// 			if (UrlSuccess(wwwNotes))
				// 				notes = wwwNotes.text;
				// 
				// 			wwwNotes = null;
				// 
				// 			if (notes != "")
				// 				window.Repaint();
				// 		}
				// 
				// 		EditorApplication.update -= Update;
				// 	}

				// FIXED VERSION:
