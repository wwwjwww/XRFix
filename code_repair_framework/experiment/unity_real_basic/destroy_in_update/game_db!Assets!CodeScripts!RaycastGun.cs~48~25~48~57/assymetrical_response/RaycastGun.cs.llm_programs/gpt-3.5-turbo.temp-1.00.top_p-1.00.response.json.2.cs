using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LineRenderer))]
public class RaycastGun : MonoBehaviour
{
    public Camera playerCamera;
    public Transform laserOrigin;
    public GameObject Object;
    private bool isPlayerOnObject = false;
    public GameObject player;
    public float gunRange = 50f;
    public float fireRate = 0.2f;
    public float laserDuration = 0.05f;
 
    LineRenderer laserLine;
    float fireTimer;

    AudioManager audioManager;
    public List<GameObject> destroyObject; // List of objects to destroy

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }




void Update()
{
    if (isPlayerOnObject)
    {
        fireTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.K) && fireTimer > fireRate)
        {
            fireTimer = 0;
            ShootLaser();
        }
    }
}

IEnumerator DestroyObject(GameObject obj)
{
    destroyObject.Add(obj); // Add the object to the list of objects to be destroyed
    yield return new WaitForSeconds(laserDuration); // Wait for the laser duration
    foreach (GameObject go in destroyObject)
    {
        Destroy(go); // Destroy the objects in the list
    }
    destroyObject.Clear(); // Clear the list
    Object = null; // Destroyed object
}


 
    IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerOnObject = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerOnObject = false;
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}