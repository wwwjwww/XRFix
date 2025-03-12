using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5f;
    public string button = "Sprint";
	




    void Start() {
        if (player == null) player = GameObject.Find("Player");
        if (launchObject == null) launchObject = GameObject.Find("LaunchObject");
    }

    void Update() {
        if (Input.GetButton(button)) { 
            Launch();
        }
    }

    void Launch() {
        GameObject obj = Instantiate(launchObject, transform.position, transform.rotation) as GameObject;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.AddForce(player.transform.forward * force, ForceMode.Impulse);
        }
    }


}
