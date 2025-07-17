using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start() {

        playerRigidbody = GetComponent<Rigidbody>();
    }




void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // 좌우 방향키 입력

        float moveVertical = Input.GetAxis("Vertical"); // 앞뒤 방향키 입력

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical); // 벡터3 타입으로 이동 방향 생성

        playerRigidbody.AddForce(movement * speed); // 리지드바디 컴포넌트에 힘을 더해 이동
    }
}


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
