using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;



    
    void Start()
    {
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    




public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;

    private void Start()
    {
        rb10 = GetComponent<Rigidbody>();
        gobj2 = GameObject.Find("GObject");
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void Update()
    {
        // You have not provided a valid reason for this if statement.
        // Please remove it if you meant to keep it.
        if (instantiate_gobj)
        {
            GameObject gameObj = GameObject.Instantiate(gobj2);
            gameObj.transform.position = new Vector3(Random.Range(-6, 6), 0, Random.Range(-6, 6));
            gameObj.transform.Rotate(new Vector3(0, 1, 0), Random.Range(0, 360));
        }

        // You have not provided a valid reason for this if statement.
        // Please remove it if you meant to keep it.
        if (timer <= 0)
        {
            rb10.AddForce(force);
        }

        if (Input.GetButton("Jump") && !Input.GetButton("Crouch"))
        {
            rb10.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetButtonDown("Crouch") && Input.GetButtonDown("Jump"))
        {
            rb10.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            a2.SetActive(true);
        }
        if (Input.GetButtonDown("Crouch") && !Input.GetButtonDown("Jump"))
        {
            a2.SetActive(true);
        }

        timer -= Time.deltaTime;
    }
}

}
