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
    SetLaserOriginPosition();    

    if (isPlayerOnObject && Input.GetButton("Fire1") && Time.time > fireTimer)
    {
        RaycastHit hit;
        if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out hit, gunRange))
        {
            Debug.Log("Hit " + hit.transform.gameObject.name);
            if (hit.transform.gameObject.name == "ObjectName")
            {
                Destroy(hit.transform.gameObject);
                audioManager.Play("ObjectDestroyed");
            }
        }
        fireTimer = Time.time + fireRate;
    }
}

void SetLaserOriginPosition()
{
    laserLine.SetPosition(0, laserOrigin.position);
    laserLine.SetPosition(1, laserOrigin.position + laserOrigin.forward * gunRange);
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