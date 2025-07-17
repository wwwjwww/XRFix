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





Constructors<DynamicBlockHit> createBall=new DiagramStaticBlockHit(hit.point.x, hit.point.y, hit.point.z, hit.point.w, hit.point.y, hit.point.w);Constructors<DynamicBlockHit> unblockHit=UnityARMatrixOps.GetUnityARMatrix(hit.worldTransform);

Constructors	CreateBall(new Vector3(unblockHit.x, unblockHit.y + createHeight, unblockHit.point.z)); Constructors<DynamicBlockHit> createBall=new DiagramStaticBlockHit(hit.worldTransform.x, hit.worldTransform.y, hit.worldTransform.z);Constructors<DynamicBlockHit> unblockHit=UnityARMatrixOps.GetUnityARMatrix(hit.worldTransform);

Constructors	CreateBall(new Vector3(unblockHit.x, unblockHit.y + createHeight, unblockHit.point.z)); 


}
