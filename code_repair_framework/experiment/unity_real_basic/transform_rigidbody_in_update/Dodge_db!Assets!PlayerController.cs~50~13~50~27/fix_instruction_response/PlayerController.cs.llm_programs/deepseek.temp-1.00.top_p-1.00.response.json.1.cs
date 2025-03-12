using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start() {

        playerRigidbody = GetComponent<Rigidbody>();
    }

///     void Update() {
// 
//         float xInput = Input.GetAxis("Horizontal");
//         float zInput = Input.GetAxis("Vertical");
// 
// 
//         float xSpeed = xInput * speed;
//         float zSpeed = zInput * speed;
// 
// 
//         Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);
// 
//         playerRigidbody.velocity = newVelocity;  
//         
//         
// 
// 
// 
//         if (Input.GetKey(KeyCode.UpArrow) == true) {
// 
//             playerRigidbody.AddForce(0f, 0f, speed);
//         }
// 
//         if (Input.GetKey(KeyCode.DownArrow) == true) {
// 
//             playerRigidbody.AddForce(0f, 0f, -speed);
//         }
// 
//         if (Input.GetKey(KeyCode.RightArrow) == true) {
// 
//             playerRigidbody.AddForce(speed, 0f, 0f);
//         }
// 
//         if (Input.GetKey(KeyCode.LeftArrow) == true) {
// 
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             playerRigidbody.AddForce(-speed, 0f, 0f);
            //         }
            //     }

            // FIXED CODE:


void FixedUpdate() {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;

        Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);

        playerRigidbody.velocity = newVelocity;  

        if (Input.GetKey(KeyCode.UpArrow) == true) {
            playerRigidbody.AddForce(0f, 0f, speed);
        }

        if (Input.GetKey(KeyCode.DownArrow) == true) {
            playerRigidbody.AddForce(0f, 0f, -speed);
        }

        if (Input.GetKey(KeyCode.RightArrow) == true) {
            playerRigidbody.AddForce(speed, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.LeftArrow) == true) {
            playerRigidbody.AddForce(-speed, 0f, 0f);
        }
    }

In Unity, `void Update()` is generally used for updating properties or states over time, while `FixedUpdate()` is used for physics calculations and movements. According to Unity's official document, `FixedUpdate()` is called at a fixed interval and is independent of frame rate. This is more suitable for implementing movements based on physics. Therefore, to keep the player's movement synced with the physics engine, you should call forces or changes to Rigidbody's velocity in `FixedUpdate()` method.


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
