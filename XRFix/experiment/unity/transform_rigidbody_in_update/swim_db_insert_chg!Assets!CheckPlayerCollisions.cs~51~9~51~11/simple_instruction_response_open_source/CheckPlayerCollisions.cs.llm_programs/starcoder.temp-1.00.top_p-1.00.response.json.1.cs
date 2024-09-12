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





The first step in learning how to code is to choose a programming language that you would like to learn. There are many different programming languages to choose from, each with their own advantages and disadvantages. Some popular languages include Python, JavaScript, Java, and C++. Once you have chosen a language, you can start learning the basics by completing tutorials or watching tutorials on YouTube. It is also a good idea to find some beginner-friendly projects to work on and try to complete them as

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
