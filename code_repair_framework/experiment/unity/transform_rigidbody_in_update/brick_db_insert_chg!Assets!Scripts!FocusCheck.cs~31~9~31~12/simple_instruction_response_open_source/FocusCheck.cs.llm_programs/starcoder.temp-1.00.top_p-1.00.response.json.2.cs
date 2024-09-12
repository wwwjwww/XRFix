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

    void Start()
    {
        rb10 = GetComponentInChildren<Rigidbody>();
        gobj2 = Instantiate(Resources.Load<GameObject>("Prefabs/Cube"), transform.position, transform.rotation);
        a2 = gobj2.GetComponent<Animation>();
        a2.Play();
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void Update()
    {
        if (timer > timeLimit)
        {
            if (instantiate_gobj == false)
            {
                instantiate_gobj = true;
                Instantiate
}
