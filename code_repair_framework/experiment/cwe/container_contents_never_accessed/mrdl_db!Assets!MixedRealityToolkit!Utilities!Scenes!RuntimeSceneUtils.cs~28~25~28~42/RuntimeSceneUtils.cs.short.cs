using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

        public static bool FindScene(string sceneName, out Scene scene, out int sceneIndex)
        {
            // BUG: Container contents are never accessed
            // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
            //             List<Scene> allScenesInProject = new List<Scene>();

            // FIXED CODE:
