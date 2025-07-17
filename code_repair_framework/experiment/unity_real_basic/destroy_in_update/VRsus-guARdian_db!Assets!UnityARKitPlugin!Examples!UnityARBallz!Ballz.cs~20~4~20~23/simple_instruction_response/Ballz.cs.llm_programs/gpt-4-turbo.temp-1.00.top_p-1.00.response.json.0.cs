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
			// 			Destroy (gameObject);
			// 		}
			// 	}

			// FIXED CODE:


void Update () {
        if (Mathf.Abs (startingY - transform.position.y) > yDistanceThreshold) {
            Destroy (gameObject);
        }
    }


}
