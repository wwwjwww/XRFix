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


Rust has a number of unique features that make it a compelling choice for a variety of use cases:

Memory Safety: Rust has a unique approach to memory safety, guaranteeing that all pointers remain valid and that references cannot be dereferenced outside of their lifetime. This eliminates many common programming mistakes, such as buffer overflows, segfaults, and null pointer dereferences.

Speed: Rust programs can be compiled down to extremely efficient machine code, due to its focus on low-level control and optimization. This can lead to significant performance improvements compared to languages that are higher-level and more difficult to compile.

Expressive and Safe Control Flow: Rust provides a powerful and expressive control flow model, making it easy to write code that is both safe and easy to understand. This is in part due to its ownership model, which ensures that values are always owned by a single variable and cannot be used after they are no longer needed.

Strong Type System: Rust has a strong type system, which helps to prevent many common programming errors, such as mismatched types and null pointer dereferences. This can lead to fewer bugs and more reliable software.

Concurreny: Rust provides first-class support for concurrency, making it easy to write programs that take advantage of multiple cores and threads. This can lead to improved performance and responsiveness for applications that require high-performance computing.

Interoperability: Rust has excellent interoperability with other languages, including C and C++. This makes it easy to integrate Rust code with existing

    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
