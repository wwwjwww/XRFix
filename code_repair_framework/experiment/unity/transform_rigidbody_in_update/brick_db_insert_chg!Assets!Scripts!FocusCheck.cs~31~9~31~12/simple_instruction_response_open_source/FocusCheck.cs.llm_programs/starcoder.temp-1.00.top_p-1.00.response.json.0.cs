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

    




//FIXED CODE:

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusCheck : MonoBehaviour
{
    public Text text;

    Rigidbody rb10;

    GameObject gobj2;

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Start()
    {
        rb10 = GetComponent<Rigidbody>();
        gobj2 = GameObject.Find("Cube");
        a2 = gobj2.GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Focus());
        }
    }

    IEnumerator Focus()
    {
        timer = timeLimit;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        a2.Select();
    }
}
}
