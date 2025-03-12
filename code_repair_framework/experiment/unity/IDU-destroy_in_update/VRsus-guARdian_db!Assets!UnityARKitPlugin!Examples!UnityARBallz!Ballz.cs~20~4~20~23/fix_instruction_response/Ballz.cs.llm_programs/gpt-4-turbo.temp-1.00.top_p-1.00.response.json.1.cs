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


    private bool markedForDestruction = false;

    void Update () {
        if (!markedForDestruction && Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold) {
            markedForDestruction = true;
            StartCoroutine(DestroyAfterFrame());
        }
    }

    private IEnumerator DestroyAfterFrame() {
        yield return null; // Wait for the end of the current frame
        Destroy(gameObject);
    }


}
