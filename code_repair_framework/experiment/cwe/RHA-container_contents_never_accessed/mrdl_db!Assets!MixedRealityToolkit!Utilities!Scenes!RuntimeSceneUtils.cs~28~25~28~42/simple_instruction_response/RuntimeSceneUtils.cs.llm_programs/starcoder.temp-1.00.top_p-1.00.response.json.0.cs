


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
            
            
            
            
            
Fixed Code:

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
            var scenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < scenes; i++)
            {
                var loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (loadedScene.name == sceneName)
                {
                    scene = loadedScene;
                    sceneIndex = i;
                    return true;
                }
            }

            scene = default;
            sceneIndex = -1;
            return false;
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