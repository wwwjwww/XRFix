//Here're the fixed code lines from /Assets/CheckPlayerCollisions.cs:
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

    protected Rigidbody rb4;

    private void Start()
    {
        rb = GetComponent<Rigidbody();
        InvokeRepeating("CheckTimeAndInstantiate", 0, timeLimit);
    }

    private void CheckTimeAndInstantiate()
    {
        if (!instantiate_gobj)
        {
            a7 = Instantiate(gobj7);
            var component7 = a7.AddComponent<HandManager>();
                component7.RemoveObject();
            timer = 0;
            instantiate_gobj = true;
        }
        else
        {
            Destroy(a7);
            timer = 0;
            instantiate_gobj = false;
        }

        rb4.transform.Rotate(new Vector3(10, 0, 0));

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

    private void HideStatus()
    {
        statusText.SetActive(false);
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
