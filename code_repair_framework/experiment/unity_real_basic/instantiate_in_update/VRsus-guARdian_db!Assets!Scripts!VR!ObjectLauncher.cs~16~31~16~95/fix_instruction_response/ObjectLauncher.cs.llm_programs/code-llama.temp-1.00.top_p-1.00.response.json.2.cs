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


    void Awake() {
        // Build an object pool here
    }

    void Update() {
        if (Input.GetButtonDown(button)) {
            // Instantiate an object from the pool
            GameObject temp = ObjectPool.Instance.GetObject(launchObject);
            // Set the transform, rotation and velocity of the instantiated object
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.GetComponent<Rigidbody>().velocity = transform.forward * force;
            // Set the Player and button properties of the instantiated object
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
        }
    }


}
