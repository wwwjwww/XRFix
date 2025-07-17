// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//	void Update () {
//		#if UNITY_EDITOR   //we will only use this script on the editor side, though there is nothing that would prevent it from working on device
//		if (Input.GetMouseButtonDown (0)) 
//		{
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//			RaycastHit hit;
//
//
//
//			if (Physics.Raycast (ray, out hit, maxRayDistance, collisionLayer)) 
//			{
//				CreateBall (new Vector3 (hit.point.x, hit.point.y + createHeight, hit.point.z));
//
//
//				Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hit.point.x, hit.point.y, hit.point.z));
//			}
//		}
//		#else
//		if (Input.touchCount > 0 )
//		{
//			var touch = Input.GetTouch(0);
//			if (touch.phase == TouchPhase.Began)
//			{
//				var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
//				ARPoint point = new ARPoint {
//					x = screenPosition.x,
//					y = screenPosition.y
//				};
//						
//				List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, 
//					ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
//				if (hitResults.Count > 0) {
//					foreach (var hitResult in hitResults) {
//						Vector3 position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
//						CreateBall (new Vector3 (position.x, position.y + createHeight, position.z));
//						break;
//					}
//				}
//
//			}
//		}
//		#endif
//
//	}

// FIXED CODE:
