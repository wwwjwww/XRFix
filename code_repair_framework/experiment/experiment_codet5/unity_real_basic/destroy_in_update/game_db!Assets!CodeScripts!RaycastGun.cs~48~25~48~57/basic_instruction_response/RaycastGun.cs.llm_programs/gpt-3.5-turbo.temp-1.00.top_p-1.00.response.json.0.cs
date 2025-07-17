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

///     void Update()
//     {
//         if (isPlayerOnObject)
//         {
//             fireTimer += Time.deltaTime;
//             if (Input.GetKeyDown(KeyCode.K) && fireTimer > fireRate)
//             {
//                 fireTimer = 0;
//                 laserLine.SetPosition(0, laserOrigin.position);
//                 Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
//                 RaycastHit hit;
//                 if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, gunRange))
//                 {
//                     if (hit.collider.gameObject == Object)
//                     {
//                         audioManager.PlaySFX(audioManager.swordSound);
//                         laserLine.SetPosition(1, hit.point);
//                         audioManager.PlaySFX(audioManager.disappearSound);
                        // BUG: Destroy in Update() method
                        // MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
                        //                         Destroy(hit.transform.gameObject);
                        //                         Object = null; // Destroyed object
                        //                     }
                        //                 }
                        //                 else
                        //                 {
                        //                     laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * gunRange));
                        //                 }
                        //                 StartCoroutine(ShootLaser());
                        // 
                        // 
                        //                 if (Object == null)
                        //                 {
                        //                     GameOver();
                        //                 }
                        //             }
                        //         }
                        //     }

                        // FIXED CODE:


void Update()
    {
        if (isPlayerOnObject)
        {
            fireTimer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.K) && fireTimer > fireRate)
            {
                fireTimer = 0;
                StartCoroutine(ShootLaser());
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