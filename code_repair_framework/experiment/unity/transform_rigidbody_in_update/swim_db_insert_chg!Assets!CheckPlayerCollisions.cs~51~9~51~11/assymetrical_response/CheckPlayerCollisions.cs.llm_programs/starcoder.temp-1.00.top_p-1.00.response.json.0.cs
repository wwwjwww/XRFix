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

/*     private void Update()
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
        * BUG: Transform object of Rigidbody in Update() methods
        * MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
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

        * Move this function in FixedUpdate() methods.
        * FIXED CODE:
        */

        
FIXED CODE:

using UnityEngine;

public class CheckPlayerCollisions : MonoBehaviour
{
    private Rigidbody rb;
    public float ripForce = 50f;
    public GameObject deathText;
    public GameObject winText;

    protected GameObject gobj7;
    protected Rigidbody rb4;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (timer >= timeLimit)
        {
            Instantiate(winText, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        timer += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            Instantiate(winText, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(deathText, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Pickup"))
        {
            Destroy(other.gameObject);
            Instantiate_gobj = true;
        }
    }

    private void FixedUpdate()
    {
        if (instantiate_gobj)
        {
            Instantiate(gobj7, new Vector3(Random.Range(-5f, 5f), Random.Range(5f, 10f), Random.Range(-5f, 5f)), transform.rotation);
            instantiate_gobj = false;
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
