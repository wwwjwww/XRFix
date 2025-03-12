using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




            Vector3 velocity = player.transform.forward * force;

            Rigidbody rb = launchObject.AddComponent<Rigidbody>();


}
