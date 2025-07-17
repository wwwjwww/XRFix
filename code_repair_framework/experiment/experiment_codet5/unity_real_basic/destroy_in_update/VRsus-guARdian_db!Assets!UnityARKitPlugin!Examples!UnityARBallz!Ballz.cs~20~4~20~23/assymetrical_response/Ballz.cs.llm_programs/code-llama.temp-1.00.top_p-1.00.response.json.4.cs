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
		if (transform.position.y - startingY > yDistanceThreshold) {
			transform.position = new Vector3(transform.position.x, startingY - yDistanceThreshold, transform.position.y);
		}
	}


}
