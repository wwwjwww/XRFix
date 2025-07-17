using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballz : MonoBehaviour {

    public float yDistanceThreshold;

    private float startingY;


    void Start () {
        startingY = transform.position.y;
    }
	




    private bool shouldDestroy;

    void Update () {
        if (!shouldDestroy && Mathf.Abs(startingY - transform.position.y) > yDistanceThreshold) {
            shouldDestroy = true;
            StartCoroutine(DestroyAfterFrame());
        }
    }

    IEnumerator DestroyAfterFrame() {
        yield return null; // Wait for the end of the frame
        Destroy(gameObject);
    }


}
