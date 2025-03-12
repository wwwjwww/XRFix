using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5f;
    public string button = "Fire1"; // Fire1 is the default button for the space bar
	




    private Rigidbody rb;

    private void Start() {
        rb = launchObject.GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetButtonDown(button)) {
            LaunchObject();
        }
    }

    void LaunchObject() {
        if(rb != null){
            rb.AddForce(transform.forward * force, ForceMode.Impulse);
        }
    }


}
