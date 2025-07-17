using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

	public float yDistanceThreshold;

	private float startingY;


	void Start () {
		startingY = transform.position.y;
	}
	




	private bool shouldBeDestroyed = false;

	void Update () {
		if (!shouldBeDestroyed && Mathf.Abs (startingY - transform.position.y) > yDistanceThreshold) {
			shouldBeDestroyed = true;
		}

		if (shouldBeDestroyed) {
			Destroy (gameObject);
		}
	}


}
