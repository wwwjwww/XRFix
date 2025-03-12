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
    float horizontalInput = Input.GetAxis("Horizontal"); // 이동 입력값
    transform.position += transform.right * speed * horizontalInput * Time.deltaTime; // 이동
}

void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
        playerRigidbody.AddForce(Vector3.up * 10f, ForceMode.Impulse); // 점프
    }
}



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
