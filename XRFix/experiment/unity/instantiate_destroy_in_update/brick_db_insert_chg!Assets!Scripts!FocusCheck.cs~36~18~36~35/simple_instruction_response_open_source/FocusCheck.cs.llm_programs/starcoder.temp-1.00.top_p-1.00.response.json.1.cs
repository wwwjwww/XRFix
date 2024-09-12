//FIXED CODE

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

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;
    private GameObject a2;

    private void Awake()
    {
        rb10 = GetComponent<Rigidbody>();
        gobj2 = Resources.Load<GameObject>("Prefab/Cube");
    }

    private void Start()
    {

    }

    void Update()
    {
        if (OVRManager.hasInputFocus && OVRManager.hasVrFocus)
        {
            rb10.transform.Translate(4, 0, Time.deltaTime);

            timer += Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                a2 = Instantiate(gobj2);
                timer = 0;
                instantiate_gobj = true;
            }
            if (instantiate_gobj && timer >= timeLimit)
            {
                Destroy(a2);
                timer = 0;
                instantiate_
}
