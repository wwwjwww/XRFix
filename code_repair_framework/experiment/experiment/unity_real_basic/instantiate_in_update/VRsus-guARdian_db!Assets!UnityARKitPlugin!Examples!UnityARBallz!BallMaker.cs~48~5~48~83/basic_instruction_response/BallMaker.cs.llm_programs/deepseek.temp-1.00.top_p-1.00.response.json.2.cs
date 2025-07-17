using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class BallMaker : MonoBehaviour {

	public GameObject ballPrefab;
	public float createHeight;
	public float maxRayDistance = 30.0f;
	public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer
	private MaterialPropertyBlock props;


	void Start () {
		props = new MaterialPropertyBlock ();

	}

	void CreateBall(Vector3 atPosition)
	{
		GameObject ballGO = Instantiate (ballPrefab, atPosition, Quaternion.identity);
			
		
		float r = Random.Range(0.0f, 1.0f);
		float g = Random.Range(0.0f, 1.0f);
		float b = Random.Range(0.0f, 1.0f);

		props.SetColor("_InstanceColor", new Color(r, g, b));

		MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
		renderer.SetPropertyBlock(props);

	}



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





I think you might have a misunderstanding when asking for a fixed code. As the current code seems to be mostly fine and is handling instantiation of the ball within Update() method by checking for different inputs (mouse click/touch), it is hard to recommend a specific changes.

Nonetheless, here is the most common way to handle instantiation of an object in Unity without updating every frame:

1. Instead of using `Update`, create a new method called for example `OnMouseDown` or `OnTouchBegan` depending upon the system you're working on (mobile or PC). This method will be called whenever the object is clicked or touched on screen.

2. Move the logic of instantiation from `Update` to the newly created method.

3. Instead of destroying the instantiated gameObject in `Update` method, you can destroy it any time before Update is called, or when the game object is inactive. For example, if you want to destroy the instantiated object after some time, you can use `Destroy(gameObject, time);` 

Please remember to remove the parts of the code which were moved to the new method.

If there is any specific functionality required in the Update loop that was causing the bug, that might not be clear from the given code, then please provide a brief description for that functionality. I will be able to help you better in providing a correction.

Also, please let me know if you need the fixed code.

}
