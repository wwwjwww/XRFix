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
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             playerRigidbody.AddForce(speed, 0f, 0f);
            //         }
            // 
            //         if (Input.GetKey(KeyCode.LeftArrow) == true) {
            // 
            //             playerRigidbody.AddForce(-speed, 0f, 0f);
            //         }
            //     }

            // FIXED CODE:


public void FixedUpdate() {
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

This FixedUpdate() method is used specifically for physics simulations. It is better to use FixedUpdate() for physics related operations because it executes at a fixed interval rather than every frame, independent of frame rate. So, you should place the physics related operations (like AddForce and velocity assignment) inside the FixedUpdate() method. The "bug" message in your code is telling you to do the same.


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
