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
        // 플레이어의 움직임을 처리합니다.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);

        playerRigidbody.AddForce(movement * speed, ForceMode.Impulse);

        // 이동에 필요한 아래의 명령문을 추가합니다.
        Vector3 newPosition = playerRigidbody.position + new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
        playerRigidbody.position = newPosition;
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
