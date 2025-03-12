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
        ballPool = new GameObject[ballPoolSize]; // Initialize the ball pool array
        
        // Instantiate balls
        for (int i = 0; i < ballPoolSize; i++) {
            ballPool[i] = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            ballPool[i].SetActive(false); // Deactivate all balls at the start
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





    private GameObject[] ballPool; // Pool of ball instances

    private int ballPoolSize = 10; // Number of balls to instantiate

    private int currentBall = 0; // Current active ball

    void CreateBall(Vector3 atPosition)
    {
        // Get next ball in the pool
        GameObject ballGO = ballPool[currentBall];
        
        // Set position and activate
        ballGO.transform.position = new Vector3(atPosition.x, atPosition.y + createHeight, atPosition.z);
        ballGO.SetActive(true);

        // Update color
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        props.SetColor("_InstanceColor", new Color(r, g, b));

        MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
        renderer.SetPropertyBlock(props);

        currentBall++; // Move to next ball

        if (currentBall >= ballPoolSize) {
            currentBall = 0; // Reset to first ball when the pool is full
        }
    }



}
