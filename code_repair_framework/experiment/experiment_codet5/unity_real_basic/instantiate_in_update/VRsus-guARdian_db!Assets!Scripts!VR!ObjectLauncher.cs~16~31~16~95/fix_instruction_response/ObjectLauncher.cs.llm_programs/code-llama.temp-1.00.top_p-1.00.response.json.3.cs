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
		// If the button is pressed down
		if (Input.GetButtonDown(button)) {
            // Instantiate the launch object outside of Update() method
			GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);

			// Set the player on the launch object
			temp.GetComponent<Launchable>().Player = player;

			// Set the button on the launch object
			temp.GetComponent<Launchable>().button = button;

			// Apply force to the launch object
			temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
		}
	}


}
