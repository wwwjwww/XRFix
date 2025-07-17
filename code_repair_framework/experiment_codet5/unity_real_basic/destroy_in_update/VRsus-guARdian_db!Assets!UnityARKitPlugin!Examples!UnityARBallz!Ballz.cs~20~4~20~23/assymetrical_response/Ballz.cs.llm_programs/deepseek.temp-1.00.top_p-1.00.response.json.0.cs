using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

public float yDistanceThreshold = 10.0f;

	private float startingY;


	void Start () {
		startingY = transform.position.y;
	}
	




void Update () {
    float currentY = transform.position.y;
    float verticalDistance = Mathf.Abs(startingY - currentY);

    if (verticalDistance > yDistanceThreshold) {
        Debug.Log(gameObject.name + " has travelled " + verticalDistance + " units vertically.");
    }
}


}
