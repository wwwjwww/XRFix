using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb;
    protected GameObject gobj2;
    private GameObject a2;

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Non-physics updates should be done here
        text.text = $"Input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }

    void FixedUpdate()
    {
        // Physics updates should be done here
        rb.MovePosition(rb.position + new Vector3(4, 0, Time.fixedDeltaTime * 60));
        
        timer += Time.fixedDeltaTime;
        
        if (!instantiate_gobj && timer >= timeLimit){
            a2 = Instantiate(gobj2);
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit ){
            Destroy(a2);
            instantiate_gobj = false;
        }

        // Reset timer here according to the instantiate_gobj status
        if ((instantiate_gobj && timer >= timeLimit) || (!instantiate_gobj && timer >= timeLimit))
        {
            timer = 0;
        }
    }
}

