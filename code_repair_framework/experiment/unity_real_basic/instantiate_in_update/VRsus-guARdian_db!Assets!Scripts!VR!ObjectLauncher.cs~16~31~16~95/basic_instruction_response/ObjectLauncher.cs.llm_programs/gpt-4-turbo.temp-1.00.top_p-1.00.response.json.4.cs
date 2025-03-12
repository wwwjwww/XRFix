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
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            //             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            //             temp.GetComponent<Launchable>().Player = player;
            //             temp.GetComponent<Launchable>().button = button;
            //         }
            // 	}

            // FIXED CODE:


    void Update () {
        if (Input.GetButtonDown(button))
        {
            LaunchObject();
        }
    }

    private void LaunchObject()
    {
        GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
        Rigidbody rb = temp.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * force, ForceMode.Impulse);
        }

        Launchable launchable = temp.GetComponent<Launchable>();
        if (launchable != null)
        {
            launchable.Player = player;
            launchable.button = button;
        }
    }


}
