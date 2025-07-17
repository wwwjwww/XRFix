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

			// you can try to build an object pool before Update() method has been called.
			// FIXED CODE:


    private bool isBelowThreshold = false; // Flag to check if the ball has already been marked for destruction

    void Update () {
        if (!isBelowThreshold && Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold) {
            isBelowThreshold = true; // Marking the ball for destruction
            StartCoroutine(DestroyAfterSeconds(0.1f)); // Start a coroutine to destroy the object
        }
    }

    IEnumerator DestroyAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }


}
