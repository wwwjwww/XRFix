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
		float currentY = this.transform.position.y;
		float yDistance = Mathf.Abs(currentY - startingY);
		if (yDistance > yDistanceThreshold) {
			print(yDistance);
		}
	}


}
