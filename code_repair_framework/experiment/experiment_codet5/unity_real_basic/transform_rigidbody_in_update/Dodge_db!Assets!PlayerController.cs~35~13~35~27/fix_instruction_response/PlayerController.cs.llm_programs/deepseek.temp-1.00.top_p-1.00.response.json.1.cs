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
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
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

The Update() method in Unity is called every frame (the delta time between frames being the time in seconds it took to render the last frame) and could cause performance issues if you're not careful.

On the other hand, FixedUpdate() is called at a fixed interval (at the fixed frame rate). Use FixedUpdate() if you're dealing with physics, as it's reliable for simulating rigid bodies. Plus, you're controlling the player move with arrow keys, it's not a perfect fit for FixedUpdate().

Also note, if you'd like to move the object by add an upwards force to the RigidBody on the UP ARROW key, you would have to change the parameters in the AddForce() method from (0f, 0f, speed) to (0f, speed, 0f).


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
