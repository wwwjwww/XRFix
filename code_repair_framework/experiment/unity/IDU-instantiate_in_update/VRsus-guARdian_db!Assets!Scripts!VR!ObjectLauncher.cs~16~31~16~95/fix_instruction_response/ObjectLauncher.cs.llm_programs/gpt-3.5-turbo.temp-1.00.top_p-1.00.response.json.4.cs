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

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


private void Start()
{
    // Create object pool
    for (int i = 0; i < 10; i++)
    {
        GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
        temp.SetActive(false);
    }
}

void Update ()
{
    if (Input.GetButtonDown(button))
    {
        // Get object from pool
        GameObject temp = GetPooledObject();
        if (temp != null)
        {
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            // Set object active
            temp.SetActive(true);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
        }
    }
}

private GameObject GetPooledObject()
{
    for (int i = 0; i < transform.childCount; i++)
    {
        GameObject obj = transform.GetChild(i).gameObject;
        if (!obj.activeInHierarchy)
        {
            return obj;
        }
    }
    return null;
}


}
