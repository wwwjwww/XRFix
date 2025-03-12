using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start() {

        playerRigidbody = GetComponent<Rigidbody>();
    }




void FixedUpdate() {
    float xInput = Input.GetAxis("Horizontal");
    float zInput = Input.GetAxis("Vertical");

    Vector3 moveDirection = new Vector3(xInput, 0, zInput).normalized;
    playerRigidbody.velocity = moveDirection * speed;
} 
}


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
