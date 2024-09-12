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

        
public void Update()
{
    // Hide the status text after 5 seconds
    if (timer > timeLimit && statusText != null)
    {
        HideStatus();
    }

    // Set the instantiate_gobj variable to true when the player collides with the gobj7 object
    if (a7.transform.GetChild(7).transform.position == Vector3.zero)
    {
        instantiate_gobj = true;
    }

    // Instantiate a GameObject when instantiate_gobj is true
    if (instantiate_gobj == true)
    {
        Instantiate(gobj7, a7.transform.GetChild(7).transform.position, Quaternion.identity);
        instantiate_gobj = false;
    }
}

// Fixed code snippet
private void FixedUpdate()
{
    // Apply a force on the rigidbody in the opposite direction of the player's velocity
    rb.AddForce(Vector3.zero, ForceMode.Impulse);

    // Check if the player has collided with the gobj7 object
    if (Physics.CheckSphere(transform.position, 5f) == true)
    {
        // Instantiate the winText GameObject and set its text to "You Win"
        Instantiate(winText, transform.position, Quaternion.identity);

        // Pause the game
        Time.timeScale = 0f;
    }

    // Check if the player has collided with the death zone
    if (Physics.CheckSphere(transform.position, 10f) == true)
    {
        // Instantiate the deathText GameObject and set its text to "You Lose"
        Instantiate(deathText, transform.position, Quaternion.identity);

        // Stop the game
        Application.Quits();
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
