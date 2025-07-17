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
        // Check if the player is within range of the gun
        if (Vector3.Distance(transform.position, player.position) < gunRange)
        {
            // Check if it is time to fire
            if (Time.time >= fireTimer)
            {
                // Set the fire timer for the next time the gun can fire
                fireTimer = Time.time + fireRate;

                // Create a raycast from the laser origin to the player
                Ray ray = new Ray(laserOrigin.position, player.position - laserOrigin.position);

                // Check if the ray intersects with any objects in the scene
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, gunRange))
                {
                    // Check if the hit object is in the destroyObject list
                    if (destroyObject.Contains(hitInfo.transform.gameObject))
                    {
                        // Play a sound effect
                        audioManager.PlaySound("laser_shoot");

                        // Set the color of the laser line based on the hit object
                        laserLine.SetColors(Color.red, Color.green);

                        // Disable the object
                        hitInfo.transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        // Play a different sound effect
                        audioManager.PlaySound("laser_no_hit");

                        // Set the color of the laser line based on the hit object
                        laserLine.SetColors(Color.red, Color.blue);
                    }
                }
                else
                {
                    // Play a different sound effect
                    audioManager.PlaySound("laser_no_hit");

                    // Set the color of the laser line based on the hit object
                    laserLine.SetColors(Color.red, Color.blue);
                }
            }
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