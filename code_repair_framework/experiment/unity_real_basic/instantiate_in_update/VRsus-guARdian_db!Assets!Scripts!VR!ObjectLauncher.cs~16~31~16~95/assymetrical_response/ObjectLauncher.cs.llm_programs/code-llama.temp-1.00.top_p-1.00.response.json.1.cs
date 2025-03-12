using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    public KeyCode button = KeyCode.S;

    void Update() {
        if (Input.GetKeyDown(button)) {
            Rigidbody launchRigidbody = launchObject.GetComponent<Rigidbody>();
            launchRigidbody.AddForce(force * player.transform.forward, ForceMode.Impulse);
        }
    }


}
