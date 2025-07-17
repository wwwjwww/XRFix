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
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerRigidbody.AddForce(Vector3.forward * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            playerRigidbody.AddForce(Vector3.back * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            playerRigidbody.AddTorque(Vector3.left * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            playerRigidbody.AddTorque(Vector3.right * speed * Time.deltaTime);
        }
    }

    void Update()
    {
        if (Input.GetAxis("Vertical") > 0f)
        {
            playerRigidbody.AddForce(Vector3.forward * speed * Time.deltaTime);
        }
        if (Input.GetAxis("Vertical") < 0f)
        {
            playerRigidbody.AddForce(Vector3.back * speed * Time.deltaTime);
        }
        if (Input.GetAxis("Horizontal") < 0f)
        {
            playerRigidbody.AddTorque(Vector3.left * speed * Time.deltaTime);
        }
        if (Input.GetAxis("Horizontal") > 0f)
        {
            playerRigidbody.AddTorque(Vector3.right * speed * Time.deltaTime);
        }
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
