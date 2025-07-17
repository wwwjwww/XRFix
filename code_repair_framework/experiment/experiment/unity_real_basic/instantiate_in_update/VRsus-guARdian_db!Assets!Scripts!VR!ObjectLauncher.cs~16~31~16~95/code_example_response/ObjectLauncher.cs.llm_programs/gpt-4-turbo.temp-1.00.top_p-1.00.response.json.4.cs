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


    private List<GameObject> objectPool;

    public int poolSize = 10;

    void Start() {
        objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(launchObject);
            obj.SetActive(false);
            obj.GetComponent<Launchable>().Player = player;
            obj.GetComponent<Launchable>().button = button;
            objectPool.Add(obj);
        }
    }

    void Update () {
        if (Input.GetButtonDown(button)) {
            GameObject temp = GetPooledObject();
            if (temp != null) {
                temp.transform.position = transform.position;
                temp.transform.rotation = transform.rotation;
                temp.SetActive(true);
                temp.GetComponent<Rigidbody>().velocity = Vector3.zero; // Ensure no leftover velocity
                temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            }
        }
    }

    GameObject GetPooledObject() {
        for (int i = 0; i < objectPool.Count; i++) {
            if (!objectPool[i].activeInHierarchy) {
                return objectPool[i];
            }
        }

        GameObject obj = Instantiate(launchObject);
        obj.SetActive(false);
        obj.GetComponent<Launchable>().Player = player;
        obj.GetComponent<Launchable>().button = button;
        objectPool.Add(obj);
        Debug.LogWarning("Expanded object pool. Consider increasing initial pool size.");
        return obj;
    }


}
