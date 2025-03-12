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


    private bool shouldDestroy = false;

    void Update () {
        if (!shouldDestroy && Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold) {
            shouldDestroy = true;
            StartCoroutine(DestroyAfterFrame());
        }
    }

    private IEnumerator DestroyAfterFrame() {
        yield return null; // Wait for the current frame to end
        Destroy(gameObject);
    }


}
