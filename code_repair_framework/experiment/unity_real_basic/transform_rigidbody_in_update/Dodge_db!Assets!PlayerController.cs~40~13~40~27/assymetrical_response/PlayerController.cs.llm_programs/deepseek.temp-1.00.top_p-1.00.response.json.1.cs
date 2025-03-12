using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start() {

        playerRigidbody = GetComponent<Rigidbody>();
    }




        float xInput = Input.GetAxis("Horizontal"); // x축 입력을 받아옴

        float zInput = Input.GetAxis("Vertical"); // z축 입력을 받아옴

        Vector3 direction = transform.TransformDirection(new Vector3(xInput, 0, zInput));



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
