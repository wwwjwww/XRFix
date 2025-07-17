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
        if (Input.GetButtonDown("Fire1") && Time.time > fireTimer)
        {
            Shoot();
            fireTimer = Time.time + 1 / fireRate;
        }
    }

    void Shoot()
    {
        Vector3 rayOrigin = laserOrigin.position;
        Vector3 rayDirection = playerCamera.transform.forward;
        Ray ray = new Ray(rayOrigin, rayDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, gunRange))
        {
            if (hit.transform.tag == "Enemy")
            {
                audioManager.Play("Laser");
                hit.transform.GetComponent<Enemy>().TakeDamage(50);
                Destroy(hit.transform.gameObject);
                Vector3 endPos = hit.point;
                laserLine.SetPosition(0, rayOrigin);
                laserLine.SetPosition(1, endPos);
                Instantiate(Object, endPos, Quaternion.identity);
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