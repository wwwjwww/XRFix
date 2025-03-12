using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start() {

        playerRigidbody = GetComponent<Rigidbody>();
    }




주어진 코드는 Unity에서 PlayerController 스크립트를 사용하여 캐릭터가 주어진 속도로 이동할 수 있는 기능을 구현합니다. 이 스크립트는 PlayerController 게임 오브젝트에 추가되어야 합니다. Start() 메소드는 playerRigidbody를 컴포넌트로 가져오고, FixedUpdate() 메소드는 입력에 따라 이동을 구현합니다.



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
