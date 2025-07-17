using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

	public float yDistanceThreshold;

	private float startingY;


	void Start () {
		startingY = transform.position.y;
	}
	




void Update () {
		if (Mathf.Abs(transform.position.y - startingY) > yDistanceThreshold) {
			Vector3 newPosition = transform.position;
			newPosition.y = startingY + Mathf.Sign(transform.position.y - startingY) * yDistanceThreshold;
			transform.position = newPosition;
		}
	}
}

}
