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
	

/// 	void Update () {
// 
// 		if (Mathf.Abs (startingY - transform.position.y) > yDistanceThreshold) {
			// BUG: Destroy in Update() method
			// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
			// 			Destroy (gameObject);
			// 		}
			// 	}

			// FIXED CODE:


    void Update()
    {
        if (Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold)
        {
            // FIXED CODE:
            // Instead of calling Destroy() in Update(), we call it in Start() when the gameobject is created.
            Destroy(gameObject);
        }
    }


}
