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
    float inputX = Input.GetAxis("Horizontal"); // X축 입력 값을 inputX 변수에 할당
    float inputZ = Input.GetAxis("Vertical"); // Z축 입력 값을 inputZ 변수에 할당

    // 실제 이동 방향을 입력값과 카메라 방향의 外적 벡터로 구함
    Vector3 moveDirection = (transform.forward * inputZ + transform.right * inputX).normalized;

    // 실제 이동에 방향만 사용(높이 무시)
    playerRigidbody.velocity = new Vector3(moveDirection.x * speed, playerRigidbody.velocity.y, moveDirection.z * speed);
}

This script will allow a player gameObject to move based on the input axis "Horizontal" and "Vertical". The player's speed can be adjusted with the "speed" public float variable. The script gets the Rigidbody component and uses it to move the player gameObject around.


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
