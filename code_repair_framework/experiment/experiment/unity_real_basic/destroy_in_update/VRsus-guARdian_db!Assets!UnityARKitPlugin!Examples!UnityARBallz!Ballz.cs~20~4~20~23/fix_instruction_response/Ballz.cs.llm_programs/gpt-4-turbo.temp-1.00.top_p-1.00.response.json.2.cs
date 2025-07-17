using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

	public float yDistanceThreshold;

	private float startingY;


	void Start () {
		startingY = transform.position.y;
		isMarkedForDestruction = false;
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


	private bool isMarkedForDestruction;

	void Update () {
		if (!isMarkedForDestruction && Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold) {
			StartCoroutine(DestroyAfterDelay());
			isMarkedForDestruction = true;
		}
	}

	private IEnumerator DestroyAfterDelay() {
		yield return new WaitForEndOfFrame();
		Destroy(gameObject);
	}


}
