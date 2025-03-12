using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5.0f;
    public string button = "Fire1";
	

/// 	void Update () {
// 		if (Input.GetButtonDown(button))
//         {
            //             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            //             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            //             temp.GetComponent<Launchable>().Player = player;
            //             temp.GetComponent<Launchable>().button = button;
            //         }
            // 	}

            // FIXED CODE:


    void Update() 
    {
        if (Input.GetButtonDown(button))
        {
            Launch();
        }
    }

    void Launch()
    {
        GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
        Rigidbody rb = temp.GetComponent<Rigidbody>();
       // Assuming that Launch method exists in Launchable script and direction is determined so direction can be replaced with the required direction (eg: Vector3.forward, Vector3.back, etc.)
        temp.GetComponent<Launchable>().Launch(Vector3.forward, force); 
    }


}
