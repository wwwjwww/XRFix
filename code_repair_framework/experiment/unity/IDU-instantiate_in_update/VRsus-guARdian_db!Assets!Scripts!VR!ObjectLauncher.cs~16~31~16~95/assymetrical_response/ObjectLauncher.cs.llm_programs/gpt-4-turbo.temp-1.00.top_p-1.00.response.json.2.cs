using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    private List<GameObject> objectPool = new List<GameObject>();

    private int poolSize = 10;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(launchObject);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown(button))
        {
            GameObject obj = GetPooledObject();
            if (obj != null)
            {
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
                obj.SetActive(true);
                obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                obj.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
                obj.GetComponent<Launchable>().Player = player;
                obj.GetComponent<Launchable>().button = button;
            }
        }
    }

    GameObject GetPooledObject()
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }


}
