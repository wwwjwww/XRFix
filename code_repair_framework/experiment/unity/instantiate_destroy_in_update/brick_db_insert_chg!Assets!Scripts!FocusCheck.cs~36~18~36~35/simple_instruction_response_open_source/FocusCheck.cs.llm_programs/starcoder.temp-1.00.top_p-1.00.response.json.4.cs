//Here're the buggy code lines from /Assets/Scripts/FocusCheck.cs:
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
    private List<GameObject> objList = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = new GameObject("FocusCheck_" + i);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            objList.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb10.transform.Translate(4, 0, Time.deltaTime);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit
}
