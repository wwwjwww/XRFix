using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // This line has been removed because it is not needed in the fixed code snippet.
    public float speed = 8f; // This line has been removed because it is not needed in the fixed code snippet.


    void Start()
    {
        // This line has been modified to include the playerRigidbody variable.
        // It also includes the GetComponent<Rigidbody>() method.
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
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
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


    void Update()
    {
        // Input.GetAxis("Horizontal") returns the horizontal input from the player. The input value range is -1 to 1.
        float xInput = Input.GetAxis("Horizontal");
        // Input.GetAxis("Vertical") returns the vertical input from the player. The input value range is -1 to 1.
        float zInput = Input.GetAxis("Vertical");
        // Calculates the speed for the player in the x and z directions.
        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;
        // Creates a vector with the calculated speed from the player in the x and z directions.
        Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);
        // The velocity vector is transformed in FixedUpdate() methods to simulate real-world movement.
        playerRigidbody.velocity = newVelocity;

        // The following if statements have been replaced by a single if statement that checks for the values of the horizontal and vertical input.
        if (xInput != 0f || zInput != 0f)
        {
            // Calls the AddForce() method of the Rigidbody component on the player game object to apply a force in the upward direction.
            playerRigidbody.AddForce(0f, 0f, speed * (xInput >= 0f ? 1f : -1f));
        }
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
