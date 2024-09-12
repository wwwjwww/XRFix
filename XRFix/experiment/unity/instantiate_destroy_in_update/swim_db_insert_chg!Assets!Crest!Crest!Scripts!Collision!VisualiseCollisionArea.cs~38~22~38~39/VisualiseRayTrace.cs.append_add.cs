
        void Update()
        {
            if (OceanRenderer.Instance == null || OceanRenderer.Instance.CollisionProvider == null)
            {
                return;
            }

            // Even if only a single ray trace is desired, this still must be called every frame until Trace() returns true
            _rayTrace.Init(transform.position, transform.forward);
            if (_rayTrace.Trace(out float dist))
            {
                var endPos = transform.position + transform.forward * dist;
                Debug.DrawLine(transform.position, endPos, Color.green);
                VisualiseCollisionArea.DebugDrawCross(endPos, 2f, Color.green, 0f);
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + transform.forward * 50f, Color.red);
            }
        }
    }
}
