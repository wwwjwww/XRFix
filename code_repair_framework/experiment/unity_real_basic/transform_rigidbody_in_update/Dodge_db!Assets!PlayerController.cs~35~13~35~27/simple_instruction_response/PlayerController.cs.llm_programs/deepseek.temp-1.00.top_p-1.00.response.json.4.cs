using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody;
    public float speed = 8f;


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
            //             playerRigidbody.AddForce(-speed, 0f, 0f);
            //         }
            //     }

            // FIXED CODE:


    private Vector2 input;

    void FixedUpdate() {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        input = Vector2.ClampMagnitude(input, 1f);
        playerRigidbody.velocity = new Vector3(input.x * speed, playerRigidbody.velocity.y, input.y * speed);
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
