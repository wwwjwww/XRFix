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
            launchObject.GetComponent<Rigidbody>().AddForce(player.transform.forward * force, ForceMode.Impulse);
        }
    }


}
