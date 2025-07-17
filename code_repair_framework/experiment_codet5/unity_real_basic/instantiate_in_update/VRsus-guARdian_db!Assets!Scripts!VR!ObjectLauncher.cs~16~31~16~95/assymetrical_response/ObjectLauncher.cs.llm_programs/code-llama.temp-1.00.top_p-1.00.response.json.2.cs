using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    void Update() {
        if (Input.GetButton(button)) {
            // Fixed code: Check if the player is on the ground before launching the object
            if (player.GetComponent<GroundCheck>().isGround()) {
                Vector3 forceDirection = player.transform.forward * force;
                launchObject.GetComponent<Rigidbody>().AddForce(forceDirection);
            }
        }
    }


}
