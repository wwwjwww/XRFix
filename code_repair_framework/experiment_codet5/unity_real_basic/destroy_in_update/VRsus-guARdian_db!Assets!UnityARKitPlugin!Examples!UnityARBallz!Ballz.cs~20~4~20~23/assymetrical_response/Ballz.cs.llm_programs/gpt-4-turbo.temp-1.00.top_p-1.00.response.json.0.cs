using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

	public float yDistanceThreshold;

	private float startingY;


	void Start () {
		startingY = transform.position.y;
	}
	




	private bool isDestroyed = false;

	void Update () {
		if (!isDestroyed && Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold) {
			Destroy(gameObject);
			isDestroyed = true;
		}
	}


}
