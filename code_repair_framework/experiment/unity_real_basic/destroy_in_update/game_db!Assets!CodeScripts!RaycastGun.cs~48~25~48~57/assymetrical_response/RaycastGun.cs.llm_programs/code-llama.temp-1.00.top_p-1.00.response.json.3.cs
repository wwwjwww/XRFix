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
    float dist = gunRange;

    RaycastHit hit;
    if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, dist))
    {
        Vector3 endPos = hit.point;
        laserLine.SetPosition(1, endPos);

        // Check if we hit an object
        if (hit.collider.gameObject != null)
        {
            // Check if the object we hit is in the destroyObject list
            if (destroyObject.Contains(hit.collider.gameObject))
            {
                // Destroy the object
                Destroy(hit.collider.gameObject);
            }
        }
    }
    else
    {
        laserLine.SetPosition(1, laserOrigin.transform.position + playerCamera.transform.forward * dist);
    }
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