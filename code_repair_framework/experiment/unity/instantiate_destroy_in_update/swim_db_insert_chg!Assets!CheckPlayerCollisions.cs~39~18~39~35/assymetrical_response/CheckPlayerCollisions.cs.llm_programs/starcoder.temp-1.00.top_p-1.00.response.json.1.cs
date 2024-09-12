
/* Here're the buggy code lines from /Assets/CheckPlayerCollisions.cs:*/
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

/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     private void Update()
*     {
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit)
*         {
*             a7 = Instantiate(gobj7);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit )
*         {
*             var component7 = a7.AddComponent<HandManager>();
*             component7.RemoveObject();
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
*         rb4.transform.Rotate(10, 0, 0);
* 
*         if (Input.GetKeyDown(KeyCode.DownArrow))
*         {
*             ripForce -= 10;
*             statusText.SetActive(true);
*             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
*             Invoke("HideStatus", 2);
*         }
*         else if (Input.GetKeyDown(KeyCode.UpArrow))
*         {
*             ripForce += 10;
*             statusText.SetActive(true);
*             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
*             Invoke("HideStatus", 2);
*         }
*     }
/* */
Thomas Edison: Hi Nikola! I just wanted to say congratulations on your work with electricity! I saw the light bulb you created and it was incredible.

Nikola Tesla: Oh thank you Thomas! I'm glad you were able to see it. It's been a long time in the making.

Edison: So Nikola, what's next for you? Do you have any plans to improve on my light bulb?

Tesla: Actually Thomas, I have been working on a way to transmit electricity over long distances. I think I might have found a solution that could make it much more efficient.

Edison: Hmm, I'm not sure I see the advantage in that. What's wrong with the current system we have?

Tesla: Well, the current system uses a lot of energy to transmit electricity over long distances

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
