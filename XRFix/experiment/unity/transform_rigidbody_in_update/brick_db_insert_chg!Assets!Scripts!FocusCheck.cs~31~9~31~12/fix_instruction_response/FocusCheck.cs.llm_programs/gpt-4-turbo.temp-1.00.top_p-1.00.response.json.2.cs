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
    
    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Start()
    {
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a2);
            timer = 0;
            instantiate_gobj = false;
        }
        else if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }

        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }

    void FixedUpdate()
    {
        // Move the Rigidbody transformations to the FixedUpdate method
        rb10.MovePosition(rb10.position + new Vector3(4, 0, 0) * Time.fixedDeltaTime);
    }
}

