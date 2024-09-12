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

    private void Start()
    {
        rb10 = GetComponent<Rigidbody>();
        gobj2 = GameObject.Find("Cube (2)");
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void Update()
    {
        if (!instantiate_gobj)
        {
            if (EventSystem.current.currentSelectedGameObject == gobj2)
            {
                Instantiate(gobj2, new Vector3(0, 5, 0), Quaternion.identity);
                Instantiate(gobj2, new Vector3(5,
}
