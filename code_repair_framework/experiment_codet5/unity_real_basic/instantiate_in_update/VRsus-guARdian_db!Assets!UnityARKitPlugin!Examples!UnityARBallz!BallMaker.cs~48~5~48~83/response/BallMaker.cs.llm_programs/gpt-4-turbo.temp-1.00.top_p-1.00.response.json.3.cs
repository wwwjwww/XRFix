using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class BallMaker : MonoBehaviour {

    public GameObject ballPrefab;
    public float createHeight;
    public float maxRayDistance = 30.0f;
    public LayerMask collisionLayer = 1 << 10; // ARKitPlane layer
    private MaterialPropertyBlock props;


    void Start() {
        props = new MaterialPropertyBlock ();
        InitializeBallPool();
    }


//	void CreateBall(Vector3 atPosition)
//	{
//		GameObject ballGO = Instantiate (ballPrefab, atPosition, Quaternion.identity);
//			
//		
//		float r = Random.Range(0.0f, 1.0f);
//		float g = Random.Range(0.0f, 1.0f);
//		float b = Random.Range(0.0f, 1.0f);
//
//		props.SetColor("_InstanceColor", new Color(r, g, b));
//
//		MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
//		renderer.SetPropertyBlock(props);
//
//	}





    private Queue<GameObject> ballPool;

    public int poolSize = 10;

    void InitializeBallPool() {
        ballPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++) {
            GameObject ballGO = Instantiate(ballPrefab, Vector3.one * 1000, Quaternion.identity); // Place far away
            ballGO.SetActive(false);
            ballPool.Enqueue(ballGO);
        }
    }

    GameObject GetBallFromPool() {
        if (ballPool.Count > 0) {
            GameObject ball = ballPool.Dequeue();
            ball.SetActive(true);
            return ball;
        }

        return Instantiate(ballPrefab);
    }

    void ReturnBallToPool(GameObject ball) {
        ball.SetActive(false);
        ball.transform.position = Vector3.one * 1000; // Place far away
        ballPool.Enqueue(ball);
    }

    void CreateBall(Vector3 atPosition) {
        GameObject ballGO = GetBallFromPool();
        ballGO.transform.position = atPosition;
        
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        props.SetColor("_InstanceColor", new Color(r, g, b));

        MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(props);
    }

    void Update() {
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer)) {
                CreateBall(new Vector3(hit.point.x, hit.point.y + createHeight, hit.point.z));
                Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hit.point.x, hit.point.y, hit.point.z));
            }
        }
        #else
        if (Input.touchCount > 0) {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
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
