using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




void Update()
    {
        if (Input.GetButtonDown(button))
        {
            // Instantiate object to be launched
            GameObject newObject = Instantiate(launchObject, player.transform.position, player.transform.rotation);

            // Add forward force
            Rigidbody rb = newObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(player.transform.forward * force, ForceMode.Impulse);
            }
        }
    }


}
