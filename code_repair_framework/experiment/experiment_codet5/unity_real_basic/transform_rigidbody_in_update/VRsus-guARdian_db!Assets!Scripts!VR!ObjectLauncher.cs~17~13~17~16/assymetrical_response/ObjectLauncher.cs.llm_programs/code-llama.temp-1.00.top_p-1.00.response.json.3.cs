using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5.0f;
    public string button = "Sprint";
	




    void Update()
    {
        if (Input.GetButton(button) && player.GetComponent<Rigidbody>().velocity.sqrMagnitude < 0.01f)
        {
            launchObject.GetComponent<Rigidbody>().AddForce(player.transform.forward * force, ForceMode.Impulse);
        }
    }


}
