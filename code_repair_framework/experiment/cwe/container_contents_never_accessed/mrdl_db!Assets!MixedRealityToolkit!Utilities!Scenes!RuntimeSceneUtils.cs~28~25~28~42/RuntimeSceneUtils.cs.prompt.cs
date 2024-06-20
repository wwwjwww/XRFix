﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class RuntimeSceneUtils
    {
        public static string GetSceneNameFromScenePath(string scenePath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        /// <summary>
        /// Finds a scene in our build settings by name.
        /// </summary>
        public static bool FindScene(string sceneName, out Scene scene, out int sceneIndex)
        {
            // BUG: Container contents are never accessed
            // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
            //             scene = default(Scene);
            //             sceneIndex = -1;
            //             // This is the only method to get all scenes (including unloaded)
            //             // This absurdity is necessary due to a long-standing Unity bug
            //             // https://issuetracker.unity3d.com/issues/scenemanager-dot-getscenebybuildindex-dot-name-returns-an-empty-string-if-scene-is-not-loaded
            //             List<Scene> allScenesInProject = new List<Scene>();
            //             for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            //             {
            //                 string pathToScene = SceneUtility.GetScenePathByBuildIndex(i);
            //                 string checkSceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
            //                 if (checkSceneName == sceneName)
            //                 {
            //                     scene = SceneManager.GetSceneByBuildIndex(i);
            //                     sceneIndex = i;
            //                     return true;
            //                 }
            //             }
            //             return false;
            //         }

            // FIXED VERSION:
