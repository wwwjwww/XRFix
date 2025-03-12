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


    void Start()
    {
        props = new MaterialPropertyBlock();
        // build an object pool for the ball prefab
        for (int i = 0; i < 10; i++)
        {
            GameObject ballGO = Instantiate(ballPrefab, transform);
            ballGO.SetActive(false);
            ballGO.name = "Ball " + i;
        }
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
        // Use an inactive ball from the object pool
        GameObject ballGO = transform.Find("Ball").gameObject;
        ballGO.SetActive(true);
        ballGO.transform.position = atPosition;

        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        props.SetColor("_InstanceColor", new Color(r, g, b));

        MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(props);
    }



}
