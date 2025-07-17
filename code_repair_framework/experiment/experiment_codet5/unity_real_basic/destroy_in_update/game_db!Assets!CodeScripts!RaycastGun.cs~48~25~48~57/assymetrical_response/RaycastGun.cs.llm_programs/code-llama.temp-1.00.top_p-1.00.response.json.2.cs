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




    List<GameObject> destroyObject; // List of objects to destroy

    void Update()
    {   
        if (Input.GetButton("Fire1") && Time.time > fireTimer)   // Fixed code
        {
            fireTimer = Time.time + fireRate;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, gunRange)) 
            { 
                Object = hit.transform.gameObject;
                isPlayerOnObject = true;
                laserLine.SetPosition(0, laserOrigin.position);
                laserLine.SetPosition(1, hit.point);
                laserLine.gameObject.SetActive(true);
                AudioManager audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
                audioManager.Play("GunShot");
            }
            else
            {
                isPlayerOnObject = false;
                laserLine.gameObject.SetActive(false);
            }
        }
    }

    void OnGUI() //Add a GUI for shooting objects.
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "Shoot"))
        {
            // Raycast to the mouse position
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, gunRange))
            {
                Object = hit.transform.gameObject;
                isPlayerOnObject = true;
                laserLine.SetPosition(0, laserOrigin.position);
                laserLine.SetPosition(1, hit.point);
                laserLine.gameObject.SetActive(true);
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