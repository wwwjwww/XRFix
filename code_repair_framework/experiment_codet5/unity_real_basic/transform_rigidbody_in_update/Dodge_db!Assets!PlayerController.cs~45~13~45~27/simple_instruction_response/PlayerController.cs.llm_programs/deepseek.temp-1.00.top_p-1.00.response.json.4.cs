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
            //             playerRigidbody.AddForce(-speed, 0f, 0f);
            //         }
            //     }

            // FIXED CODE:


    void FixedUpdate() {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // 좌표로 이동속도를 생성
        Vector3 moveDirection = new Vector3(xInput, 0, zInput).normalized; 
        // normalized를 사용해 이동속도가 1이 되도록 방향 조정

        // 이동속도를 적용
        playerRigidbody.velocity = moveDirection * speed; 

        // 위의 velocity 방식과 같은 효과를 얻는 다른 방법
        // playerRigidbody.AddForce(moveDirection * speed);
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
