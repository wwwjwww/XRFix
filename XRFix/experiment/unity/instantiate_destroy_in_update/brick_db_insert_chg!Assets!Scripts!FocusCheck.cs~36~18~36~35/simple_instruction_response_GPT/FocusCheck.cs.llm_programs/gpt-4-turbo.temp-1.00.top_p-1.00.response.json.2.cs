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
        InvokeRepeating("InstantiateOrDestroyObject", timeLimit, timeLimit);
    }

    void Update()
    {
        rb10.transform.Translate(4 * Time.deltaTime, 0, 0);

        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }

    private void InstantiateOrDestroyObject()
    {
        if (!instantiate_gobj)
        {
            a2 = Instantiate(gobj2);
            instantiate_gobj = true;
        }
        else
        {
            Destroy(a2);
            instantiate_gobj = false;
        }
    }
}
}
