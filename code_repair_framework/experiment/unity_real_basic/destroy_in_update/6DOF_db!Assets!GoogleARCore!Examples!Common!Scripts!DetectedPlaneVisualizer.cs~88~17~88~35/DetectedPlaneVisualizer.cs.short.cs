
/*         public void Update()
*         {
*             if (m_DetectedPlane == null)
*             {
*                 return;
*             }
*             else if (m_DetectedPlane.SubsumedBy != null)
*             {
                * BUG: Destroy in Update() method
                * MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
                *                 Destroy(gameObject);
                *                 return;
                *             }
                * 
                *             else if (m_DetectedPlane.TrackingState != TrackingState.Tracking || ARAnchoring.isVR)
                *             {
                *                  m_MeshRenderer.enabled = false;
                *                  return;
                *             }
                * 
                *             m_MeshRenderer.enabled = true;
                * 
                *             _UpdateMeshIfNeeded();
                *         }

                * you can try to build an object pool before Update() method has been called.
                * FIXED CODE:
                */
