using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
