


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

            //Remove or Commented-out the collection if it is no longer needed
            // FIXED CODE:
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

            if (sceneName == null || sceneName == "")
            {
                return false;
            }

            List<Scene> allScenesInProject = new List<Scene>();
            List<Scene> currentSceneInBuild = new List<Scene>();

            // Find all scenes in the project
            foreach (var path in UnityEngine.SceneManagement.SceneManager.GetScenePaths())
            {
                allScenesInProject.Add(SceneManager.GetSceneByPath(path));
            }

            // Find the current scene
            foreach (var sceneGuid in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetAllSceneNodes())
            {
                currentSceneInBuild.Add(SceneManager.GetSceneByName(sceneGuid.path));
            }

            for (int i = 0; i < allScenesInProject.Count; i++)
            {
                if (allScenesInProject[i].IsActive())
                {
                    if (allScenesInProject[i].name == sceneName)
                    {
                        scene = allScenesInProject[i];
                        sceneIndex = i;
                        return true;
                    }
                }
            }

            Debug.LogError($"Can't find scene with name {sceneName}");
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