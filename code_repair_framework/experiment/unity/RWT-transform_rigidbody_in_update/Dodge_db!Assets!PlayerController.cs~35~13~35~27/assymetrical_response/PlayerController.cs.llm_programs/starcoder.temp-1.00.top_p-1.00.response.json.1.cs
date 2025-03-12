using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start() {
        playerRigidbody = GetComponent<Rigidbody>();
    }




        float horizontal = Input.GetAxis("Horizontal"); // Input.GetAxis()은 -1 또는 1의 값으로 사용자가 조작한 컨트롤을 알려줍니다.

        float vertical = Input.GetAxis("Vertical");



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
