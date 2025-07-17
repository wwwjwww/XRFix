using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    public Rigidbody launchObject;

    void Update()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            LaunchObject();
        }
    }

    void LaunchObject()
    {
        launchObject.AddForce(player.transform.forward * force, ForceMode.Impulse);
    }


}
