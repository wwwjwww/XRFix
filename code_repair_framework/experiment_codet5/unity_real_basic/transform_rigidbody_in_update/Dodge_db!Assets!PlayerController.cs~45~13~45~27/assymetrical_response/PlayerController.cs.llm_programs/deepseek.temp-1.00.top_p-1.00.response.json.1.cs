using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start() {

        playerRigidbody = GetComponent<Rigidbody>();
    }




void Update() {
         float xInput = Input.GetAxis("Horizontal");
         float zInput = Input.GetAxis("Vertical");

         // 실제 이동 속도를 설정
         float xSpeed = xInput * speed;
         float zSpeed = zInput * speed;

         // x, z 축의 방향으로 이동
         Vector3 newVelocity = new Vector3(xSpeed, 0, zSpeed);
         playerRigidbody.velocity = newVelocity;
     }
 }


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
