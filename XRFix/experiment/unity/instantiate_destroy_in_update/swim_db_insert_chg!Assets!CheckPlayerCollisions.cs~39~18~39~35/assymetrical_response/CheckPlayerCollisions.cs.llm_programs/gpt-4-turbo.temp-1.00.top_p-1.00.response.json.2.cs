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
        StartCoroutine(ManageObjects());
    }

    private IEnumerator ManageObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeLimit);

            if (!instantiate_gobj)
            {
                a7 = Instantiate(gobj7);
                instantiate_gobj = true;
            }
            else
            {
                var component7 = a7.GetComponent<HandManager>();
                if (component7 != null)
                {
                    component7.RemoveObject();
                }
                a7 = null;  // Ensure the reference is cleared.
                instantiate_gobj = false;
            }
        }
    }
    
    private void Update()
    {
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
