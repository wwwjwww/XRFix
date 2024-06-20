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

    protected Rigidbody rb4;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }

    private void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         GameObject a7 = Instantiate(gobj7);
        //         var component7 = a7.AddComponent<HandManager>();
        //         component7.RemoveObject();
        // 
        //         rb4.transform.Rotate(10, 0, 0);
        // 
        //         if (Input.GetKeyDown(KeyCode.DownArrow))
        //         {
        //             ripForce -= 10;
        //             statusText.SetActive(true);
        //             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
        //             Invoke("HideStatus", 2);
        //         }
        //         else if (Input.GetKeyDown(KeyCode.UpArrow))
        //         {
        //             ripForce += 10;
        //             statusText.SetActive(true);
        //             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
        //             Invoke("HideStatus", 2);
        //         }
        //     }

        // FIXED VERSION:
