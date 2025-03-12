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
        float horizontalInput = Input.GetAxis("Horizontal"); // 가로축 입력 얻기
        float verticalInput = Input.GetAxis("Vertical"); // 세로축 입력 얻기

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput); // 이동 방향 Vector3 객체 생성
        playerRigidbody.velocity = movement * speed; // 이동 속력으로 이동 속도를 계산하고 적용
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
