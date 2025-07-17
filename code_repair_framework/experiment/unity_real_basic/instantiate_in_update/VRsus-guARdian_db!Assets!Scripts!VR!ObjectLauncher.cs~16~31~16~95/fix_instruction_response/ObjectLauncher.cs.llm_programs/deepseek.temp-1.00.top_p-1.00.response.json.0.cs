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


    public ObjectPooler objectPooler; // Assign this in the inspector

    void Update () {
        if (Input.GetButtonDown(button))
        {
            GameObject temp = objectPooler.GetPooledObject();
            if (temp != null)
            {
                temp.transform.position = transform.position;
                temp.transform.rotation = transform.rotation;  // I assume you are setting rotation to something else. If you are not changing rotation, you can remove this line.
                temp.SetActive(true);

                Rigidbody rb = temp.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(transform.forward * force, ForceMode.Impulse);
                }

                Launchable launchable = temp.GetComponent<Launchable>();
                if (launchable != null)
                {
                    launchable.Player = player;
                    launchable.button = button;
                }
            }
        }
    }

    public GameObject prefabToPool;

    public int pooledAmount = 20;

    List<GameObject> pooledObjects;

    void Start () {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(prefabToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject () {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        // If we run out of pooled objects, need to instantiate a new one
        GameObject obj = Instantiate(prefabToPool);
        obj.SetActive(false);
        pooledObjects.Add(obj);

        return obj;
    }


}
