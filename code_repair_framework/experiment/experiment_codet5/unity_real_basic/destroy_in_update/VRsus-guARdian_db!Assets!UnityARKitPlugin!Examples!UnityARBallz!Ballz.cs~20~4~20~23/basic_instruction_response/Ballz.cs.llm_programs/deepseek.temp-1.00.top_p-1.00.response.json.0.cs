using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

	public float yDistanceThreshold;

	private float startingY;


	void Start () {
		startingY = transform.position.y;
	}
	

/// 	void Update () {
// 
// 		if (Mathf.Abs (startingY - transform.position.y) > yDistanceThreshold) {
			// BUG: Destroy in Update() method
			// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
			// 			Destroy (gameObject);
			// 		}
			// 	}

			// FIXED CODE:


void Update () {
		if (Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold) {
			DestroyInOneFrame();
		}
	}

	private void DestroyInOneFrame()
	{
		gameObject.SetActive(false);
		// If you truly want to destroy the object, use the following line
		// Destroy(gameObject);
	} 

In this fixed code, we are using SetActive(false) instead of Destroy(gameObject), because SetActive(false) doesn't immediately destroy the GameObject, it enables or disables the GameObject. The GameObject becomes inactive for layout calculations, scripts calls, and physics, but it stays visible in the scene. If you actually want to destroy the object immediately, you could just use Destroy(gameObject), but be aware of performance cost when calling it in update, as it can destroy object in every frame where update runs.

}
