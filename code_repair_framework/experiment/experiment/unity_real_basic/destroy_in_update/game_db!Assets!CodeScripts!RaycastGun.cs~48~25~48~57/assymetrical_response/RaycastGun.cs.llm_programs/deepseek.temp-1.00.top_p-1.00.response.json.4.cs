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
    // Fire if the fireTimer is greater than fireRate
    if (fireTimer > fireRate)
    {
        fireTimer = 0f;

        // Create a ray from the laserOrigin point to the player's position
        Ray ray = new Ray(laserOrigin.position, player.transform.position - laserOrigin.position);
        RaycastHit hit;

        // If the raycast hits something within the range
        if (Physics.Raycast(ray, out hit, gunRange))
        {
            // Check if the hit object is in the list of destroyable objects
            if (destroyObject.Contains(hit.transform.gameObject))
            {
                // Destroy the object
                Destroy(hit.transform.gameObject);

                // Play a sound effect
                audioManager.Play("Hit");

                // Show a laser effect for laserDuration seconds
                StartCoroutine(ShowLaserEffect(hit.point));
            }
        }
    }
    fireTimer += Time.deltaTime;
}

IEnumerator ShowLaserEffect(Vector3 endPosition)
{
    laserLine.SetPosition(0, laserOrigin.position);
    laserLine.SetPosition(1, endPosition);

    // Set the line width based on distance from the origin
    float distance = Vector3.Distance(laserOrigin.position, endPosition);
    laserLine.startWidth = distance / 2;
    laserLine.endWidth = distance / 2;

    // Activate the line renderer
    laserLine.enabled = true;

    // Wait for the laserDuration
    yield return new WaitForSeconds(laserDuration);

    // Deactivate the line renderer
    laserLine.enabled = false;
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