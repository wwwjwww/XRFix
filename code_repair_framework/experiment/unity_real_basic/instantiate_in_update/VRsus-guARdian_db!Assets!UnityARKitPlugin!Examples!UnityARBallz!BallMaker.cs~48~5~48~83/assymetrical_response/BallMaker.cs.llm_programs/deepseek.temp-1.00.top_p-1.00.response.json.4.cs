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
        // Create the pool of balls.
        ballPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++) {
            ballPool[i] = Instantiate(ballPrefab);
            ballPool[i].SetActive(false); // Deactivate all initially.
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





    public int poolSize = 10; // Number of balls to be created when the scene starts.

    private GameObject[] ballPool; // Pool of balls.

    private int currentBall = 0; // Current ball in the pool.

    void Update() {
        // You don't need to use Instantiate in Update.
    }

    public void CreateBall(Vector3 atPosition, Color color) {
        // Find an inactive ball in the pool.
        for (int i = 0; i < poolSize; i++) {
            // If there are no more available balls (all active), then return.
            if(i == currentBall && ballPool[i].activeInHierarchy) {
                return;
            }

            if (!ballPool[i].activeInHierarchy) {
                // Set the ball's position and color, then activate it.
                ballPool[i].transform.position = atPosition;
                ballPool[i].SetActive(true);
                ballPool[i].GetComponent<MeshRenderer>().material.SetColor("_InstanceColor", color);

                // Reset the current ball and exit the loop.
                currentBall = i;
                break;
            }
        }
    }



}
