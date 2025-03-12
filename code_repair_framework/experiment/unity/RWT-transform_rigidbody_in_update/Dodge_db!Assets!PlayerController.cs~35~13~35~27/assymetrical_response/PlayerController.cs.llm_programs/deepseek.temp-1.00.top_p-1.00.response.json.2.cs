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
        // 수평축, 수직축의 입력을 감지
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // 실제 이동 속도를 저장할 변수
        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;

        // x,y,z 각각에 대해서 속도를 적용
        Vector3 newVelocity = new Vector3(xSpeed, 0, zSpeed);

        // 리지드바디의 속도에 새로운 속도를 넣어줌
        playerRigidbody.velocity = newVelocity;
    }

Additionally, you should ensure that your project settings have input set up. Under Edit > Project Settings > Input, you need to have the following:



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
