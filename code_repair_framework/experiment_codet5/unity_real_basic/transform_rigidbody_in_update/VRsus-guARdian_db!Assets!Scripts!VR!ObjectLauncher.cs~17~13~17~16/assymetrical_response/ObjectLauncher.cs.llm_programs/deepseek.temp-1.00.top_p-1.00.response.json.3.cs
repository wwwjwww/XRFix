using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




void Update () {
        if (Input.GetButton(button)) {
            GameObject launchedObject = Instantiate(launchObject, player.transform.position, Quaternion.identity);
            Rigidbody rb = launchedObject.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddForce(player.transform.forward * force, ForceMode.Impulse);
            }
        }
    }
}

}
