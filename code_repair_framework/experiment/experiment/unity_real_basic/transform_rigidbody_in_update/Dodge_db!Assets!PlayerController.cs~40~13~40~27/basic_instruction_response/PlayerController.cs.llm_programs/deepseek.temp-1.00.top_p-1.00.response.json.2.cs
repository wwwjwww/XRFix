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

这个修复后的代码将在 FixedUpdate() 方法中应用物理力，而不仅仅是在 Update() 方法中。在游戏中，Physics系统在 FixedTimestep 上运行，而不是在 Time.deltaTime 上运行，因此在 FixedUpdate() 中应用物理力会得到更精确的结果，特别是在移动对象时。这样做可能需要更少的计算资源，可以提供更稳定、流畅的游戏体验。我已经使用更准确、不受帧率影响的方向控制，因为物理和计算都依赖于 FixedUpdate().


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
