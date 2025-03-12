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


private List<GameObject> objectPool = new List<GameObject>(); // Added object pool list

void Start()
{
    for (int i = 0; i < 10; i++) // Creating 10 objects in the object pool
    {
        GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
        temp.SetActive(false);
        objectPool.Add(temp);
    }
}

void Update()
{
    if (Input.GetButtonDown(button))
    {
        GameObject temp = GetPooledObject(); // Get object from the pool
        if (temp != null)
        {
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.SetActive(true);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
        }
    }
}

GameObject GetPooledObject()
{
    for (int i = 0; i < objectPool.Count; i++)
    {
        if (!objectPool[i].activeInHierarchy)
        {
            return objectPool[i];
        }
    }
    return null;
}


}
