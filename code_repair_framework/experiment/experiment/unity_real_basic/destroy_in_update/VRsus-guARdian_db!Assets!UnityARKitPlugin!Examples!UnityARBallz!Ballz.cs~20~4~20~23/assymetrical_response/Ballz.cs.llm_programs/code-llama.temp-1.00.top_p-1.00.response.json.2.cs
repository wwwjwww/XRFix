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
		float yDistance = Math.Abs (transform.position.y - startingY);

		if (yDistance > yDistanceThreshold) {
			Debug.Log ("Ball has fallen past the yDistanceThreshold: " + yDistanceThreshold);
		}
	}


}
