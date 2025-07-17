using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

        private async Task LoadScenesInternal(
            IEnumerable<string> scenesToLoad,
            SceneType sceneType,
            SceneActivationToken activationToken = null,
            float progressOffset = 0,
            float progressTarget = 1,
            bool sceneOpInProgressWhenFinished = false)
        {
                    // BUG: Potentially dangerous use of non-short-circuit logic
                    // MESSAGE: The & and | operators do not use short-circuit evaluation and can be dangerous when applied to boolean operands. In particular, their use can result in errors if the left-hand operand checks for cases in which it is not safe to evaluate the right-hand one.
                    //                     scenesLoadedAndActivated &= (scene.IsValid() & scene.isLoaded);

                    // FIXED CODE:
