using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




private void Start()
{
    // Create object pool for launchObject
}

private void Update()
{
    if (Input.GetButtonDown(button))
    {
        // Fetch object from object pool instead of using Instantiate
        GameObject temp = FetchObjectFromPool();
        temp.transform.position = transform.position;
        temp.transform.rotation = transform.rotation;
        temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
        temp.GetComponent<Launchable>().Player = player;
        temp.GetComponent<Launchable>().button = button;
    }
}

private GameObject FetchObjectFromPool()
{
    // Implement logic to fetch object from object pool
    return null;
}


}
