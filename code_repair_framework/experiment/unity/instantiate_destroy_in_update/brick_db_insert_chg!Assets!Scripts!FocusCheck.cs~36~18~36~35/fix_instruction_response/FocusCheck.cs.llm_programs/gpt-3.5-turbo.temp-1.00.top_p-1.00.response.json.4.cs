//Here're the buggy code lines from /Assets/Scripts/FocusCheck.cs:
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
        rb10 = GetComponent<Rigidbody>(); // Initialize rb10 by getting the Rigidbody component
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void FixedUpdate() // Use FixedUpdate() for physics-related updates
    {
        rb10.MovePosition(rb10.position + new Vector3(4, 0, 0) * Time.fixedDeltaTime); // Use MovePosition for physics-based movement

        timer += Time.fixedDeltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a2);
            timer = 0;
            instantiate_gobj = false;
        }

        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }
}
