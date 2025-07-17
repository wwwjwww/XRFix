using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
	public float speed = 8f; // 이동 속력


	void Start() {
		playerRigidbody = GetComponent<Rigidbody>();
	}




		float horizontal = Input.GetAxis("Horizontal"); // 스페이스 키, 화살표 키 입력을 받음



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
