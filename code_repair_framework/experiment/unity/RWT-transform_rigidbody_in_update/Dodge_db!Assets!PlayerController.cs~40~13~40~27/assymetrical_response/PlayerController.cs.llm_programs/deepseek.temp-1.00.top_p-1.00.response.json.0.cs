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
        
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        // 실제 이동에 쓰일 방향을 조정한 값
        float moveHorizontal = inputX * speed;
        float moveVertical = inputZ * speed;

        // 앞뒤 움직임을 위한 벡터 생성
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        // 리지드바디에 오프셋을 적용한 이동속도 적용
        playerRigidbody.AddForce(movement);
    }
}


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
