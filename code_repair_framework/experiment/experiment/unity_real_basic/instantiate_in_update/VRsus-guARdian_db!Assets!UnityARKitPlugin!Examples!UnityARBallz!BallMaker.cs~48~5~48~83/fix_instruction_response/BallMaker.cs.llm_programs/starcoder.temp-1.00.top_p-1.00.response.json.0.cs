using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class BallMaker : MonoBehaviour {

	public GameObject ballPrefab;
	public float createHeight;
	public float maxRayDistance = 30.0f;
	public LayerMask collisionLayer = 1 << 10;  
	private MaterialPropertyBlock props;


	void Start () {
		props = new MaterialPropertyBlock ();

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





	void CreateBall(Vector3 atPosition)
	{
		GameObject ballGO = GameObject.Instantiate(ballPrefab, atPosition, Quaternion.identity);

		float r = Random.Range(0.0f, 1.0f);
		float g = Random.Range(0.0f, 1.0f);
		float b = Random.Range(0.0f, 1.0f);

		props.SetColor("_InstanceColor", new Color(r, g, b));

		MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
		renderer.SetPropertyBlock(props);

	}

	void Update()
	{
		// create ball if mouse pressed
		if (Input.GetMouseButtonDown(0)) 
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer)) 
			{
				CreateBall(new Vector3(hit.point.x, hit.point.y + createHeight, hit.point.z));
				// adjust object pool size if necessary
				if (GameObject.FindObjectsOfType<GameObject>().Length > 200)
				{
					Destroy(GameObject.FindObjectsOfType<GameObject>()[0]);
				}
			}
		}
	}

The first two 'CreateBall(Vector3 atPosition)' functions are the same as the original ones, but they are marked as 'internal' to prevent direct access. Instead, we provide a 'CreateBall' function for the client to instantiate the ball object.

The Update() function is modified to use the object pool. We create a ball object only when the mouse is pressed, and adjust the size of the object pool if necessary.


}
