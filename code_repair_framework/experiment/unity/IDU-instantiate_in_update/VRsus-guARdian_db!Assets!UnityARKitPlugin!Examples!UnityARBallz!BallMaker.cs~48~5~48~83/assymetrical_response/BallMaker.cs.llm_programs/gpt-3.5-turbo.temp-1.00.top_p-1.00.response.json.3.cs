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


void Start()
{
    props = new MaterialPropertyBlock();
    CreateObjectPool();
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





private void CreateObjectPool()
{
    for (int i = 0; i < 10; i++) // Example: Create 10 balls in the object pool
    {
        GameObject ballGO = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        ballGO.SetActive(false);
    }
}

void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer))
        {
            GameObject ballFromPool = GetPooledObject();
            if (ballFromPool != null)
            {
                ballFromPool.transform.position = new Vector3(hit.point.x, hit.point.y + createHeight, hit.point.z);
                ballFromPool.SetActive(true);

                Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hit.point.x, hit.point.y, hit.point.z));
            }
        }
    }
    else if (Input.touchCount > 0)
    {
        var touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
            ARPoint point = new ARPoint
            {
                x = screenPosition.x,
                y = screenPosition.y
            };

            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point,
                ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
            if (hitResults.Count > 0)
            {
                foreach (var hitResult in hitResults)
                {
                    Vector3 position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                    GameObject ballFromPool = GetPooledObject();
                    if (ballFromPool != null)
                    {
                        ballFromPool.transform.position = new Vector3(position.x, position.y + createHeight, position.z);
                        ballFromPool.SetActive(true);
                    }
                    break;
                }
            }
        }
    }
}

private GameObject GetPooledObject()
{
    foreach (GameObject obj in pooledObjects)
    {
        if (!obj.activeInHierarchy)
        {
            return obj;
        }
    }
    return null;
}



}
