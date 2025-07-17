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


有很多好书可以推荐到你对数据库的学习。这些书籍涵盖了数据库的基础，administration，design和development等多方面的内容。

1. "Database Systems: A Practical Approach to Design, Implementation and Management" By Elmasri, Navathe, and Scott。这本书是一本完整的数据库系统学习手册，包含了数据库设计、实现和管理方面的知识。

2. "Head First SQL" By Elisabeth Robson和Kevin Yank。这本书以一种有趣且亲切的方式介绍了SQL语言和数据库概念。

3. "SQL Antipatterns: Avoiding the Pitfalls of Database Design" By David Loshin。这本书从防止常见的数据库设计陷阱开始，介绍了如何使用正确的设计模式和技术来实现高效和稳定的数据库系统。

4. "Pro SQL Server 2016 Performance Tuning and Optimization" By Kalen Delaney。这本书将帮助你优化SQL Server 2016的性能，包括配置和优化实例，管理内存和I/O，以及使用内置工具和存储过程来解决性能问题。

5. "MongoDB for DBAs: The Road to Mastery" By Matias Quaranta。这本书是专门为数据库

    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
