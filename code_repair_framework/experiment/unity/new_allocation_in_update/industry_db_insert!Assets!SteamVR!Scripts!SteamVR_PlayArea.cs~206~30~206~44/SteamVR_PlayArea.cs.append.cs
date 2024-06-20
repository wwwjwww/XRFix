#endif

        void OnDrawGizmos()
        {
            if (!drawWireframeWhenSelectedOnly)
                DrawWireframe();
        }

        void OnDrawGizmosSelected()
        {
            if (drawWireframeWhenSelectedOnly)
                DrawWireframe();
        }

        public void DrawWireframe()
        {
            if (vertices == null || vertices.Length == 0)
                return;

            var offset = transform.TransformVector(Vector3.up * wireframeHeight);
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;

                var a = transform.TransformPoint(vertices[i]);
                var b = a + offset;
                var c = transform.TransformPoint(vertices[next]);
                var d = c + offset;
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(a, c);
                Gizmos.DrawLine(b, d);
            }
        }

        public void OnEnable()
        {
            if (Application.isPlaying)
            {
                GetComponent<MeshRenderer>().enabled = drawInGame;

                // No need to remain enabled at runtime.
                // Anyone that wants to change properties at runtime
                // should call BuildMesh themselves.
                enabled = false;

                // If we want the configured bounds of the user,
                // we need to wait for tracking.
                if (drawInGame && size == Size.Calibrated)
                    StartCoroutine(UpdateBounds());
            }
        }

        IEnumerator UpdateBounds()
        {
            GetComponent<MeshFilter>().mesh = null; // clear existing

            var chaperone = OpenVR.Chaperone;
            if (chaperone == null)
                yield break;

            while (chaperone.GetCalibrationState() != ChaperoneCalibrationState.OK)
                yield return null;

            BuildMesh();
        }
    }
}