

        /// <summary>
        /// Usage: destroy all created cubes and garbage collect.
        /// </summary>
        void ClearObjects()
        {
            for (int i = 0; i < spawnedCubes.Count; i++)
            {
                DestroyImmediate(spawnedCubes[i]);
            }

            spawnedCubes.Clear();
            GC.Collect();
        }

        #endregion

        #region Debug UI Handlers

        /// <summary>
        /// Usage: radio button handler.
        /// </summary>
        public void RadioPressed(string radioLabel, string group, Toggle t)
        {
            if (string.Compare(radioLabel, ovrOverlayID) == 0)
            {
                ActivateOVROverlay();
            }
            else if (string.Compare(radioLabel, applicationID) == 0)
            {
                ActivateWorldGeo();
            }
            else if (string.Compare(radioLabel, noneID) == 0)
            {
                ActivateNone();
            }
        }

        #endregion
    }
}
