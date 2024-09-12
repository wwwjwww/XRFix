FIXED CODE:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    public GameObject target;
    public float speed = 10.0f;

    void Start()
    {
        StartCoroutine(Slice());
    }

    IEnumerator Slice()
    {
        while (true)
        {
            // Wait for a few seconds
            yield return new WaitForSeconds(speed);

            // Get the current position of the target object
            Vector3 targetPos = target.transform.position;

            // Get the current position of the game object
            Vector3 myPos = transform.position;

            // Calculate the distance between the target and the game object
            float distance = Vector3.Distance(targetPos, myPos);

            // If the distance is less than 1 meter, continue slicing
            if (distance < 1.0f)
            {
                // Debug.Log("Slicing...");
            }
            else
            {
                // If the distance is greater than 1 meter, stop slicing
                break;
            }
        }
    }

}