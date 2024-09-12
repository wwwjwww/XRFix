using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCollisions : MonoBehaviour
{
    private Rigidbody rb;
    public float ripForce = 50f;
    public GameObject deathText;
    public GameObject winText;
    public GameObject statusText;

    protected GameObject gobj7;
    protected GameObject a7;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb4;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }





﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCollisions : MonoBehaviour
{
    private Rigidbody rb;
    public float ripForce = 50f;
    public GameObject deathText;
    public GameObject winText;
    public GameObject statusText;

    protected GameObject gobj7;
    protected GameObject a7;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb4;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gobj7 = GameObject.Find("Gobj7");
        a7 = GameObject.Find("A7");
        rb4 = gobj7.GetComponent<Rigidbody>();
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            statusText.GetComponent<TextMesh>().text = "Time remaining: " + timer.ToString("F1");
        }

        if (Time.timeSinceLevelLoad >= timeLimit)
        {
            if (instantiate_gobj == true)
            {
                GameObject gobj = Instantiate(gobj7, transform.position, Quaternion.identity);
                rb4.AddExplosionForce(ripForce, transform.position, 1);
                instantiate_gobj = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && instantiate_gobj == true)
        {
            rb4.AddExplosionForce(ripForce, transform.position, 1);
            rb.AddForce(transform.forward * 10);
            deathText.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space) && instantiate_gobj == false)
        {
            rb4.AddExplosionForce(ripForce, transform.position, 1);
            instantiate_gobj = true;
            statusText.SetActive(true);
        }

        if (transform.position.y < -0.5f)
        {
            winText.SetActive(true);
        }
    }
}


    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision with " + collision.name);
        if (collision.name == "DeathZone")
        {
            deathText.SetActive(true);
            winText.SetActive(false);
        } else if (collision.name == "WinZone")
        {
            winText.SetActive(true);
            deathText.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "ForceField")
        {
            rb.AddForce(collision.transform.forward * ripForce);
        }
    }
}
