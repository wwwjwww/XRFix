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
        rb10 = GetComponentInChildren<Rigidbody>();

        gobj2 = transform.Find("gobj2").gameObject;

        EventSystem.current.sendNavigationEvents = true;
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() &&!Input.GetMouseButtonDown(0))
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else
}
