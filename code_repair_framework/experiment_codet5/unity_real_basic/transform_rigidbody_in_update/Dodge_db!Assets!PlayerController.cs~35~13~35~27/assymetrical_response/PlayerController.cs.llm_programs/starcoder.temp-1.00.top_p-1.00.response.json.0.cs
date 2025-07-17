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
        float horizontal = Input.GetAxis("Horizontal"); // 가로 이동 입력
        float vertical = Input.GetAxis("Vertical"); // 세로 이동 입력
        Vector3 movement = new Vector3(horizontal, 0f, vertical); // 3축 벡터에 저장
        playerRigidbody.AddForce(movement * speed); // movement * speed를 입력하여 Rigidbody에 힘을 줌
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
