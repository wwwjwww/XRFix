//Here're the buggy code lines from /Assets/CheckPlayerCollisions.cs:
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
    private float timer = 0f;
    private bool instantiate_gobj = false;

    //protected Rigidbody rb4; // commented out as it is not being used

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }

    // FIXED: moved instantiation logic to Start() method to avoid using Instantiate/Destroy in Update()
    private void Start()
    {
        rb = GetComponent<Rigidbody();
        gobj7 = Instantiate(gobj7);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        rb.transform.Rotate(10, 0, 0); // changed rb4 to rb as rb4 is not being used

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
