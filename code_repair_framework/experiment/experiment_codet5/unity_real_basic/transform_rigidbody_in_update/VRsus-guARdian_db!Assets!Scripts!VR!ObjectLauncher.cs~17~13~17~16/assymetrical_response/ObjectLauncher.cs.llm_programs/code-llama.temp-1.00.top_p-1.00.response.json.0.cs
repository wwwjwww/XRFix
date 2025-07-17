using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    void Start()
    {
        player = GameObject.FindWithTag("Player");
        launchObject = GameObject.FindWithTag("LaunchObject");
    }

    void Update()
    {
        if (Input.GetKeyDown(button))
        {
            var launchForce = force;
            launchObject.GetComponent<Rigidbody>().AddForce(transform.forward * launchForce, ForceMode.Impulse);
        }
    }


}
