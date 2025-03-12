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

        // Pre-instantiate balls and deactivate them
        for (int i = 0; i < poolSize; i++) {
            GameObject ball = Instantiate(ballPrefab);
            ball.SetActive(false);
            ballPool.Add(ball);
        }
    }


* 	void CreateBall(Vector3 atPosition)
* 	{
* 		GameObject ballGO = Instantiate (ballPrefab, atPosition, Quaternion.identity);
* 			
* 		
* 		float r = Random.Range(0.0f, 1.0f);
* 		float g = Random.Range(0.0f, 1.0f);
* 		float b = Random.Range(0.0f, 1.0f);
* 
* 		props.SetColor("_InstanceColor", new Color(r, g, b));
* 
* 		MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
* 		renderer.SetPropertyBlock(props);
* 
* 	}





    private List<GameObject> ballPool = new List<GameObject>();

    private int poolSize = 10;  // You can adjust the size of the pool as needed

    GameObject GetPooledBall() {
        foreach (GameObject ball in ballPool) {
            if (!ball.activeInHierarchy) {
                return ball;
            }
        }
        // Expand the pool if necessary
        GameObject newBall = Instantiate(ballPrefab);
        newBall.SetActive(false);
        ballPool.Add(newBall);
        return newBall;
    }

    void CreateBall(Vector3 atPosition) {
        GameObject ballGO = GetPooledBall();
        ballGO.transform.position = atPosition;
        ballGO.transform.rotation = Quaternion.identity;
        ballGO.SetActive(true);
        
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        props.SetColor("_InstanceColor", new Color(r, g, b));

        MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(props);
    }

    void Update () {
        #if UNITY_EDITOR   //we will only use this script on the editor side, though there is nothing that would prevent it from working on device
        if (Input.GetMouseButtonDown (0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, maxRayDistance, collisionLayer)) 
            {
                CreateBall (new Vector3 (hit.point.x, hit.point.y + createHeight, hit.point.z));

                Debug.Log (string.Format ("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hit.point.x, hit.point.y, hit.point.z));
            }
        }
        #else
        if (Input.touchCount > 0 )
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint {
                    x = screenPosition.x,
                    y = screenPosition.y
                };

                List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, 
                    ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
                if (hitResults.Count > 0) {
                    foreach (var hitResult in hitResults) {
                        Vector3 position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                        CreateBall(new Vector3(position.x, position.y + createHeight, position.z));
                        break;
                    }
                }
            }
        }
        #endif
    }



}
