*         public void Update()
*         {
*             if (m_DetectedPlane == null)
*             {
*                 return;
*             }
*             else if (m_DetectedPlane.SubsumedBy != null)
*             {
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

* FIXED CODE:
*/

* you can try to build an object pool before Update() method has been called.