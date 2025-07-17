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
        float _moveHorizontal = Input.GetAxis ("Horizontal"); // "Horizontal" 입력을 x축 이동량으로
        float _moveVertical = Input.GetAxis ("Vertical"); // "Vertical" 입력을 y축 이동량으로

        Vector3 movement = new Vector3 (_moveHorizontal, 0.0f, _moveVertical); // 이동 방향을 생성
        print (_moveHorizontal);

        if (movement != Vector3.zero) { // 이동 방향이 존재하면
            //Quaternion newRotation = Quaternion.LookRotation (movement); // 캐릭터의 시선 기울임을 계산
            //playerRigidbody.rotation = newRotation; // 캐릭터의 회전을 변경
        }

        playerRigidbody.AddForce (movement * speed); // 이동 속력만큼 힘을 가함
    }
}


    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
