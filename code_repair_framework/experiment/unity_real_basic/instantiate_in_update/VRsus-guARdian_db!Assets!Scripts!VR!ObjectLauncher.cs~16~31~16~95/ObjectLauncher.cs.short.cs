using System.Collections.Generic;
using UnityEngine;

/* 	void Update () {
* 		if (Input.GetButtonDown(button))
*         {
            * BUG: Instantiate in Update() method
            * MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            *             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            *             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            *             temp.GetComponent<Launchable>().Player = player;
            *             temp.GetComponent<Launchable>().button = button;
            *         }
            * 	}

            * you can try to build an object pool before Update() method has been called.
            * FIXED CODE:
            */
