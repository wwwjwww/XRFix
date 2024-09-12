
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
//Here's the definition of function call in another component.
//Related code from /Assets/HandManager.cs:

private void Start()
{
    rb = GetComponent<Rigidbody>();
    StartObjectPooling();
}

private void StartObjectPooling()
{
    // Initialize object pooling for gobj7 here
}

private void Update()
{
    timer += Time.deltaTime;
    rb4.transform.Rotate(10, 0, 0);

    ProcessInput();
    HandleObjectPooling();
}

private void HandleObjectPooling()
{
    if (!instantiate_gobj && timer >= timeLimit)
    {
        // Get an object from the pool instead of instantiating a new one
        a7 = ObjectPooler.Instance.GetPooledObject("gobj7Tag");
        if(a7 != null) 
        {
            a7.SetActive(true);
            timer = 0;
            instantiate_gobj = true;
        }
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        // Return the object to the pool instead of destroying it
        a7.GetComponent<HandManager>().RemoveObject();
        timer = 0;
        instantiate_gobj = false;
    }
}

private void ProcessInput()
{
    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
        ripForce -= 10;
        UpdateStatusText("rip force set to " + ripForce);
    }
    else if (Input.GetKeyDown(KeyCode.UpArrow))
    {
        ripForce += 10;
        UpdateStatusText("rip force set to " + ripForce);
    }
}

private void UpdateStatusText(string text)
{
    statusText.SetActive(true);
    statusText.GetComponent<TMPro.TextMeshProUGUI>().text = text;
    Invoke("HideStatus", 2);
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
