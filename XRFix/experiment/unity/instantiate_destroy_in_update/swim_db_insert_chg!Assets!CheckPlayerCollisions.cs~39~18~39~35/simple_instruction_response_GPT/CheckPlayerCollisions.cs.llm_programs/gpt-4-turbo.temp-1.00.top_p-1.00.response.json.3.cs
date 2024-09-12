using System.Collections;
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
        StartCoroutine(ObjectCycle());
    }

    private void Update()
    {
        rb4.transform.Rotate(10, 0, 0);

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeRipForce(-10);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeRipForce(10);
        }
    }

    private IEnumerator ObjectCycle()
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
                if (a7 != null)
                {
                    Destroy(a7);
                }
                instantiate_gobj = false;
            }
        }
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }

    private void ChangeRipForce(float amount)
    {
        ripForce += amount;
        statusText.SetActive(true);
        statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
        Invoke("HideStatus", 2);
    }
}

//HandManager.cs
public class HandManager : MonoBehaviour
{
    public GameObject hand;
    public GameObject controller;
    private OVRHand ovrHand;

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }

    void Update()
    {
        if (ovrHand.IsTracked)
        {
            hand.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);
        }
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
        {
            controller.SetActive(true);
            hand.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

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
