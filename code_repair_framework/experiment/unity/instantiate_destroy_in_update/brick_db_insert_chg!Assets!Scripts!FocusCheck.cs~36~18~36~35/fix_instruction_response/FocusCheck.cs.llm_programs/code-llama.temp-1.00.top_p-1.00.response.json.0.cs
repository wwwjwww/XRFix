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

    private bool instantiate_gobj = false;

    void Start()
    {
        // Instantiate the object pool before Update() has been called.
        a2 = Instantiate(gobj2);
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    void Update()
    {
        rb10.transform.Translate(4, 0, Time.deltaTime);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            // Destroy the old game object and instantiate a new one.
            Destroy(a2);
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }

        if (instantiate_gobj && timer >= timeLimit)
        {
            // Destroy the new game object.
            Destroy(a2);
            timer = 0;
            instantiate_gobj = false;
        }

        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }
}

