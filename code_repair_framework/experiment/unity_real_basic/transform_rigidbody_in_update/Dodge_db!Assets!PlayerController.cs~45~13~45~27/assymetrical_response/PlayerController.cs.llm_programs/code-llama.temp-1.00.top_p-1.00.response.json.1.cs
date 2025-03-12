using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody playerRigidbody;
    public float speed = 8f; // 이동 속력


    void Start() {
        playerRigidbody = GetComponent<Rigidbody>();
        cursor = GetComponent<Cursor>();
    }




    private Cursor cursor;

    void Update() {
        if (cursor.lockState == CursorLockMode.Locked) {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(horizontal, 0, vertical);
            playerRigidbody.AddForce(movement * speed);
        }
    }



    public void Die() {
        // 자신의 게임 오브젝트를 비활성화
        gameObject.SetActive(false);
    }

}
