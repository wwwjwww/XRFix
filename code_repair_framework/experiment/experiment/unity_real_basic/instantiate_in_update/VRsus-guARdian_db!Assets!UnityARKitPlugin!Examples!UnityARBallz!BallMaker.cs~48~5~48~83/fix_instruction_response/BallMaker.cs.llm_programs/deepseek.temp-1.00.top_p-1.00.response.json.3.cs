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
        
        // Create a new Queue of GameObjects
        ballPool = new Queue<GameObject>();
        
        // Pre-instantiate all balls in the pool
        for(int i = 0; i < poolSize; i++) {
            GameObject ball = Instantiate(ballPrefab);
            ball.SetActive(false); // Deactivate them initially
            ballPool.Enqueue(ball); // Add them to the pool
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





    private Queue<GameObject> ballPool;

    private const int poolSize = 50; // Define the size of your pool

    void CreateBall(Vector3 atPosition)
    {
        if(ballPool.Count > 0) {
            GameObject ballGO = ballPool.Dequeue(); // Get the first ball from the pool
            ballGO.transform.position = atPosition; // Set its position
            ballGO.SetActive(true); // Activate it
            
            float r = Random.Range(0.0f, 1.0f);
            float g = Random.Range(0.0f, 1.0f);
            float b = Random.Range(0.0f, 1.0f);

            props.SetColor("_InstanceColor", new Color(r, g, b));

            MeshRenderer renderer = ballGO.GetComponent<MeshRenderer>();
            renderer.SetPropertyBlock(props);
        }
    }

    void Update () {
        // Rest of your code...
    }



}
