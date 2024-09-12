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
        rb10 = gameObject.GetComponent<Rigidbody>();
        gobj2 = GameObject.Find("GameObject2");
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeLimit && instantiate_gobj)
        {
            timer = 0;
            instantiate_gobj = false;
            GameObject gobj = Instantiate(gobj2, gameObject.transform.position, Quaternion.identity);
            a2 = gobj.GetComponent<Rigidbody>();
            a2.AddForce(gameObject.transform.forward * 10);
        }
    }
}

}
