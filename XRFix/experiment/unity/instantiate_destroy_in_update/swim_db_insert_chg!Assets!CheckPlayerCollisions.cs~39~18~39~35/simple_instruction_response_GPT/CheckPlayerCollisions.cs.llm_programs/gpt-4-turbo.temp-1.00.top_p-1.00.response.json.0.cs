
//Here're the buggy code lines from /Assets/CheckPlayerCollisions.cs:
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
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }

// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    private void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a7 = Instantiate(gobj7);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            var component7 = a7.AddComponent<HandManager>();
//            component7.RemoveObject();
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        rb4.transform.Rotate(10, 0, 0);
//
//        if (Input.GetKeyDown(KeyCode.DownArrow))
//        {
//            ripForce -= 10;
//            statusText.SetActive(true);
//            statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
//            Invoke("HideStatus", 2);
//        }
//        else if (Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            ripForce += 10;
//            statusText.SetActive(true);
//            statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
//            Invoke("HideStatus", 2);
//        }
//    }
//
// FIXED CODE:
private void Update()
{
    timer += Time.deltaTime;

    rb4.transform.Rotate(10, 0, 0);

    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
        ripForce -= 10;
        statusText.SetActive(true);
        statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
        Invoke("HideStatus", 2);
    }
    else if (Input.GetKeyDown(KeyCode.UpArrow))
    {
        ripForce += 10;
        statusText.SetActive(true);
        statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
        Invoke("HideStatus", 2);
    }
}

private void InstantiateObject()
{
    if (!instantiate_gobj)
    {
        a7 = Instantiate(gobj7);
        instantiate_gobj = true;
    }
}

private void RemoveObject()
{
    if (a7)
    {
        Destroy(a7);
        a7 = null;
        instantiate_gobj = false;
    }
}

private void CheckInstantiateOrRemove()
{
    if (!instantiate_gobj && timer >= timeLimit)
    {
        InstantiateObject();
        timer = 0;
    }
    else if (instantiate_gobj && timer >= timeLimit)
    {
        RemoveObject();
        timer = 0;
    }
}

void Start()
{
    ovrHand = hand.GetComponent<OVRHand>();
    // Added coroutine to avoid Instantiate/Destroy each frame
    StartCoroutine(InstantiateDestroyCycle());
}

// Fixed version of the coroutine to handle instantiation and destruction over time
IEnumerator InstantiateDestroyCycle()
{
    while (true)
    {
        yield return new WaitForSeconds(timeLimit);
        InstantiateObject();
        yield return new WaitForSeconds(timeLimit);
        RemoveObject();
    }
}

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;

    public void RemoveObject()
    {
        Destroy(gameObject);
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
