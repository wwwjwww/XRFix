using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	

/* 	void Update () {
* 		if (Input.GetButtonDown(button))
*         {
*             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            * BUG: Transform object of Rigidbody in Update() methods
            * MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            *             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            *             temp.GetComponent<Launchable>().Player = player;
            *             temp.GetComponent<Launchable>().button = button;
            *         }
            * 	}

            * Move this function in FixedUpdate() methods.
            * FIXED CODE:
            */
