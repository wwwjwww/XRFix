using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




void Update () {
    if(playerInput.GetButton(button)){
        GameObject newObject = Instantiate(launchObject, transform.position, transform.rotation);
        newObject.GetComponent<Rigidbody>().AddForce(transform.forward * force);
    }
}


}
