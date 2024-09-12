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
        // Initialization code here (if necessary)
    }

    void Update()
    {
        // Code in Update that is not related to physics should remain here
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2);
            timer = 0f;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a2);
            timer = 0f;
            instantiate_gobj = false;
        }

        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }

    void FixedUpdate()
    {
        // Physics-related code goes here.
        rb10.MovePosition(rb10.position + new Vector3(4, 0, 0) * Time.fixedDeltaTime);
    }
}

