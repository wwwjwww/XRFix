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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            instantiate_gobj = true;
            rb.AddForce(collision.gameObject.transform.forward * ripForce);
            collision.gameObject.transform.position = gobj7.transform.position;
            // FIXED CODE:
            // Make the "gobj7" variable null to prevent it from being referenced after it has been destroyed.
            gobj7 = null;
        }
    }

    private void OnCollisionEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            HideStatus();
            timer = 0f;
            instantiate_gobj = true;
            rb.AddForce(collision.gameObject.transform.forward * ripForce);
            collision.gameObject.transform.position = a7.transform.position;
            // FIXED CODE:
            // Make the "a7" variable null to prevent it from being referenced after it has been destroyed.
            a7 = null;
        }
    }

    private void Update()
    {
        if (instantiate_gobj)
        {
            // FIXED CODE:
            // Make the "gobj7" variable null to prevent it from being referenced after it has been destroyed.
            gobj7 = null;
            Instantiate(deathText, transform.position, transform.rotation);
            destroy(gameObject);
        }
        else if (timer > timeLimit)
        {
            // FIXED CODE:
            // Make the "a7" variable null to prevent it from being referenced after it has been destroyed.
            a7 = null;
            Instantiate(winText, transform.position, transform.rotation);
            Destroy(a7, 3f);
        }
        else
        {
            timer += Time.deltaTime;
            statusText.GetComponent<Text>().text = "Time: " + timer.ToString("F2");
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
