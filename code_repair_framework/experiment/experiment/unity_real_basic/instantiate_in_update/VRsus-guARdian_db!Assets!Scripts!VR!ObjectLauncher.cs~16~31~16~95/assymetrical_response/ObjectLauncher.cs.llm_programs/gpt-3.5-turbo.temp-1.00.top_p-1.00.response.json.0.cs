using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




private List<GameObject> objectPool = new List<GameObject>();

void Start()
{
    for (int i = 0; i < 10; i++) // Adjust the number based on your needs
    {
        GameObject obj = Instantiate(launchObject, transform.position, transform.rotation);
        obj.SetActive(false);
        objectPool.Add(obj);
    }
}

void Update()
{
    if (Input.GetButtonDown(button))
    {
        GameObject temp = GetPooledObject();
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

private GameObject GetPooledObject()
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
