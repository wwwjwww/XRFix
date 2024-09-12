using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;

    private bool instantiate_gobj = false;

    private float timeLimit = 5f;
    private float timer = 0f;

    void Start()
    {
        rb10 = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb10.transform.Translate(4 * Time.deltaTime, 0, 0);

        timer += Time.deltaTime;

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

        text.text = $"input: {OVRInput.GetActiveController()} - VR: {UnityEngine.XR.XRDevice.isPresent}";
    }
}

