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

    //실제 이동 속도를 입력값과 속력으로 계산
    float xSpeed = xInput * speed;
    float zSpeed = zInput * speed;

    //Vector3 클래스를 사용하여 속도를 각 x, y, z 축의 속도로 저장
    Vector3 newVel = new Vector3(xSpeed, 0, zSpeed);

    //리지드바디의 물리 속도를 변경
    playerRigidbody.velocity = newVel;
}

이 코드는 플레이어를 가로와 세로로 이동시킵니다. Input.GetAxis는 플레이어가 WASD 키나 조이스틱을 누르면 각각 1, 아니면 -1의 값을 반환합니다. 그 값을 speed 변수와 곱하여 최종적으로 플레이어의 이동 속도를 결정합니다.



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
