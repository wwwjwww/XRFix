using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

	public float yDistanceThreshold;

	private float startingY;


	void Start()
	{
		startingY = transform.position.y;
	}
	




	void Update()
	{
		float yDistance = transform.position.y - startingY;
		if (yDistance >= yDistanceThreshold)
		{
			transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
		}
	}


}
