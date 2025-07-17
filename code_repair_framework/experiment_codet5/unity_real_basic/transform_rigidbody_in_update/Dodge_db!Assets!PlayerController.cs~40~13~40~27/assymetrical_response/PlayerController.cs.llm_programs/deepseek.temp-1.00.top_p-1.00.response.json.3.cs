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
    float xInput = Input.GetAxis("Horizontal"); // 방향키 입력의 x축
    float zInput = Input.GetAxis("Vertical"); // 방향키 입력의 z축

    Vector3 direction = new Vector3(xInput, 0, zInput); // x축, y축, z축에 입력받은 값을 대입

    // 실제 이동은 FixedUpdate 메서드에서 함
}

void FixedUpdate()
{
    // Rigidbody의 이동 함수에 이동방향을 함께 넣어 이동을 시킴
    playerRigidbody.MovePosition(playerRigidbody.position + direction * speed * Time.fixedDeltaTime);
}



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
