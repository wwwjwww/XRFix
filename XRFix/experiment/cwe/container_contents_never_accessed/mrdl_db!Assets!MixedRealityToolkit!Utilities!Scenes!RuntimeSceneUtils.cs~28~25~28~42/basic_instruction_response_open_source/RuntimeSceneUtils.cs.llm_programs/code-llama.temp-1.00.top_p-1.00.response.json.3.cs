


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

        
        
        
        public static bool FindScene(string sceneName, out Scene scene, out int sceneIndex)
        {
            scene = default(Scene);
            sceneIndex = -1;
            
            
            
            // BUG: Container contents are never accessed
            // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
            //             List<Scene> allScenesInProject = new List<Scene>();

            // FIXED CODE:
            allScenesInProject = new List<Scene>();

            // BUG: Incorrect variable assignment
            // MESSAGE: The local variable 'assetPath' is assigned but its value is never used.
            //             var assetPath = "";

            // FIXED CODE: ﻿

 allScenesInProject = new List<Scene>();

            // BUG: Incorrect variable assignment
            // MESSAGE: The local variable 'sceneName' is assigned but its value is never used.
            //             string sceneName = "";

            // FIXED CODE: ﻿

            allScenesInProject.Add(scene);

            // BUG: Incorrect variable assignment
            // MESSAGE: The local variable 'sceneIndex' is assigned but its value is never used.
            //             int sceneIndex = -1;

            // FIXED CODE: ﻿

            var sceneIndex = -1;

            // FIXED CODE: ﻿

            scene = default(Scene);

            // BUG: Incorrect variable assignment
            // MESSAGE: The local variable 'scene' is assigned but its value is never used.
            //             var scene = default(Scene);

            // FIXED CODE: ﻿

            scene = SceneManager.GetSceneByName(sceneName);

            // BUG: Incorrect variable assignment
            // MESSAGE: The local variable 'scene' is assigned but its value is never used.
            //             var scene = SceneManager.GetSceneByName(sceneName);

            // FIXED CODE: ﻿

            scene = default(Scene);

            // FIXED CODE: ﻿

            allScenesInProject.Remove(scene);

            // FIXED CODE: ﻿

            allScenesInProject.Add(scene);

            // FIXED CODE: ﻿

            return true;
        }
    }
}
 
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string pathToScene = SceneUtility.GetScenePathByBuildIndex(i);
                string checkSceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
                if (checkSceneName == sceneName)
                {
                    scene = SceneManager.GetSceneByBuildIndex(i);
                    sceneIndex = i;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns all root GameObjects in all loaded scenes.
        /// </summary>
        public static IEnumerable<GameObject> GetRootGameObjectsInLoadedScenes()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                foreach (GameObject rootGameObject in loadedScene.GetRootGameObjects())
                {
                    yield return rootGameObject;
                }
            }
            yield break;
        }

        /// <summary>
        /// Sets the active scene to the supplied scene. Returns true if successful.
        /// </summary>
        public static bool SetActiveScene(Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return false;
            }

            try
            {
                SceneManager.SetActiveScene(scene);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }
    }
}