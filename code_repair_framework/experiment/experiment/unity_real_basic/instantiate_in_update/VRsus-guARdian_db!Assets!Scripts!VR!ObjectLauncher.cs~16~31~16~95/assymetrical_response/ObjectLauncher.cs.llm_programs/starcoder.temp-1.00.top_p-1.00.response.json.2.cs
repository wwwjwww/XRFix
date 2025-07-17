using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




            Vector3 launchVector = (player.transform.position - transform.position).normalized;

            Rigidbody rb = launchObject.GetComponent<Rigidbody>();


}
