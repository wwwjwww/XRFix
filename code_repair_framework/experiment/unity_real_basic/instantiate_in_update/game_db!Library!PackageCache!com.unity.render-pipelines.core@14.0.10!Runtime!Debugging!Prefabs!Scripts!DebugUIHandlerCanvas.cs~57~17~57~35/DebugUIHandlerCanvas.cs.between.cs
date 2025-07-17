
        internal void RequestHierarchyReset()
        {
            m_DebugTreeState = -1;
        }

        void ResetAllHierarchy()
        {
            foreach (Transform t in transform)
                CoreUtils.Destroy(t.gameObject);

            Rebuild();
        }

