using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	

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


    private Launchable launchable;

    void Start() {
        // Initialize Launchable script
        GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
        launchable = temp.GetComponent<Launchable>();
        // Set properties
        launchable.SetPlayer(player);
        launchable.SetButton(button);
    }

    void Update () {
        if (Input.GetButtonDown(button))
        {
            // Instantiate and launch
            GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            // Set properties again (since these properties may have changed during runtime)
            launchable.SetPlayer(player);
            launchable.SetButton(button);
        }
    }


}
