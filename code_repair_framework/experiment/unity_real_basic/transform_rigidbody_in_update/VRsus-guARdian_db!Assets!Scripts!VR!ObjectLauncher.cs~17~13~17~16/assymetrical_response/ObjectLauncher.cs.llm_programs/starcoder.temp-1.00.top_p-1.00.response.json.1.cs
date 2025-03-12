using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Jump";
	




    private Rigidbody rb;

    void Start()
    {
        rb = launchObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButtonDown(button))
        {
            Launch();
        }
    }


}
