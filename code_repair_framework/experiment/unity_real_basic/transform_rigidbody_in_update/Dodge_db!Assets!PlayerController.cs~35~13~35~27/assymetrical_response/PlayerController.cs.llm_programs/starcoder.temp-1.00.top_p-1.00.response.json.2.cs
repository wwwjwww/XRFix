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
        float horizontal = Input.GetAxis("Horizontal"); //水平 Input값 가져오기
        float vertical = Input.GetAxis("Vertical"); //垂直 Input값 가져오기
        Vector3 movement = new Vector3(horizontal, 0, vertical); // 3D위치를 변경하기 위해 입력값
        playerRigidbody.AddForce(movement * speed); // Input값에 따라 속도를 적용
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
