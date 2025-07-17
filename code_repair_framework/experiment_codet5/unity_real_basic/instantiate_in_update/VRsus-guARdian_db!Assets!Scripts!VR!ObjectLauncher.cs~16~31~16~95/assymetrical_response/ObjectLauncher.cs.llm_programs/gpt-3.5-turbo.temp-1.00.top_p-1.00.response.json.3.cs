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
    for (int i = 0; i < 10; i++)
    {
        GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
        temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
        temp.GetComponent<Launchable>().Player = player;
        temp.GetComponent<Launchable>().button = button;
        temp.SetActive(false);
    }
}

private void Update()
{
    if (Input.GetButtonDown(button))
    {
        GameObject temp = GetPooledObject();
        temp.transform.position = transform.position;
        temp.transform.rotation = transform.rotation;
        temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
        temp.GetComponent<Launchable>().Player = player;
        temp.GetComponent<Launchable>().button = button;
        temp.SetActive(true);
    }
}

private GameObject GetPooledObject()
{
    for (int i = 0; i < transform.childCount; i++)
    {
        GameObject temp = transform.GetChild(i).gameObject;
        if (!temp.activeInHierarchy)
        {
            return temp;
        }
    }
    return null;
}


}
