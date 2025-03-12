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


Imagining the world is only made of water, everything we know about the physical properties of matter applies, but with some slight modifications.

Objects fall faster in water than they would in air because the water displaces a larger volume, so a 1kg object will fall faster in water than it would in air. This is because the same amount of gravity pulling down on an object in water will cause it to fall through a smaller volume of water than in air. This is called the buoyancy effect. It's the reason why things float in water.



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
