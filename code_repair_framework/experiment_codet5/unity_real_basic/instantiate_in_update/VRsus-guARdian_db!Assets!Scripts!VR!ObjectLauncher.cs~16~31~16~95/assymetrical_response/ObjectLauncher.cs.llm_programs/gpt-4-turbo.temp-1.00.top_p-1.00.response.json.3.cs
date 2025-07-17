using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




void Start() {
    // Initialize object pool
    objectPool = new List<GameObject>();
}

void Update() {
    if (Input.GetButtonDown(button)) {
        // Reuse object from pool if available, otherwise instantiate a new one
        GameObject temp = GetPooledObject();
        if (temp == null) {
            temp = Instantiate(launchObject, transform.position, transform.rotation);
            AddToPool(temp);
        } else {
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.SetActive(true);
        }
        
        Rigidbody rb = temp.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.AddForce(transform.forward * force, ForceMode.Impulse);
        }
        
        Launchable launchScript = temp.GetComponent<Launchable>();
        if (launchScript != null) {
            launchScript.Player = player;
            launchScript.button = button;
        }
    }
}

private List<GameObject> objectPool;

private GameObject GetPooledObject() {
    foreach (GameObject obj in objectPool) {
        if (!obj.activeInHierarchy) {
            return obj;
        }
    }
    return null;
}

private void AddToPool(GameObject obj) {
    obj.SetActive(false);
    objectPool.Add(obj);
}


}
