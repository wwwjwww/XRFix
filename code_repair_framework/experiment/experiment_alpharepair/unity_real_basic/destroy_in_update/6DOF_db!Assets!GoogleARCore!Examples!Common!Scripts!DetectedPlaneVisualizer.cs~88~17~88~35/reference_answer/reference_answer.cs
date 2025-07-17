        private List<GameObject> planeObjectPool = new List<GameObject>();

        public void Initialize()
        {
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                GameObject planeObject = planeObjectPool[i];
                planeObject.SetActive(true);
            }
        }

        public void Update()
        {
            if (m_DetectedPlane == null)
            {
                return;
            }

            if (m_DetectedPlane.SubsumedBy != null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (m_DetectedPlane.TrackingState != TrackingState.Tracking)
            {
                m_MeshRenderer.enabled = false;
                return;
            }

            m_MeshRenderer.enabled = true;

            _UpdateMeshIfNeeded();
        }
