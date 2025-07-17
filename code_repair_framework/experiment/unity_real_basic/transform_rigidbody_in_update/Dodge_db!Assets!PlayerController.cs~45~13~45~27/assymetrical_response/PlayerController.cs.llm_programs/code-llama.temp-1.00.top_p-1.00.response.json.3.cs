using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody; // 이동에 사용할 리지드바디 컴포넌트
    public float speed = 8f; // 이동 속력


    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }




    void Update()
    {
        // 키보드 입력 감지
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 위쪽 입력 감지
            // playerRigidbody.AddForce(Vector3.up * speed);

            // 수정된 코드
            playerRigidbody.AddForce(new Vector3(0, speed, 0), ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            // 왼쪽 입력 감지
            // playerRigidbody.AddForce(Vector3.left * speed);

            // 수정된 코드
            playerRigidbody.AddForce(new Vector3(-speed, 0, 0), ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            // 아래쪽 입력 감지
            // playerRigidbody.AddForce(Vector3.down * speed);

            // 수정된 코드
            playerRigidbody.AddForce(new Vector3(0, -speed, 0), ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            // 오른쪽 입력 감지
            // playerRigidbody.AddForce(Vector3.right * speed);

            // 수정된 코드
            playerRigidbody.AddForce(new Vector3(speed, 0, 0), ForceMode.Impulse);
        }
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
