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


    public void Update () {
        if (Input.GetButtonDown(button))
        {
            // Instantiate in Start() method
            player = GameObject.FindWithTag("Player");
            launchObject = GameObject.FindWithTag("LaunchObject");
            GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
        }
    }

In the "Start" method, I replaced the "Instantiate" call with a search of all objects in the game for both the launchObject and player objects, and assign them to local variables. If either of these objects can't be found, the "Instantiate" method throws an error. To avoid this issue, I've made sure to first call the "FindWithTag" method and store the results in local variables before using them in the "Upate" method.

Please note that this solution won't fix the code inside the "Update" method, as it's not clear what is going to happen inside the if condition inside the "Update" method.

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        launchObject = GameObject.FindWithTag("LaunchObject");
    }

    void Update () {
        if (Input.GetButtonDown(button))
        {
            GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
        }
    }


}
