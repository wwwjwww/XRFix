using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

public GameObject player;
public GameObject launchObject;
public float force = 5;
public string button = "Sprint";
	




private bool canLaunch = true;

private GameObject launchObjectInstance;

void Start()
{
    launchObjectInstance = Instantiate(launchObject, transform.position, transform.rotation);
    launchObjectInstance.SetActive(false);
}

void Update()
{
    if (Input.GetButtonDown(button) && canLaunch)
    {
        LaunchObject();
    }
}

void LaunchObject()
{
    launchObjectInstance.transform.position = transform.position;
    launchObjectInstance.transform.rotation = transform.rotation;
    launchObjectInstance.SetActive(true);
    launchObjectInstance.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
    launchObjectInstance.GetComponent<Launchable>().Player = player;
    launchObjectInstance.GetComponent<Launchable>().button = button;
    canLaunch = false;
}


}
