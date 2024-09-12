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

    private Queue<GameObject> objPool;

    private void Awake()
    {
        objPool = new Queue<GameObject>();
    }

    void Start()
    {
    }

    void Update()
    {
        rb10.transform.Translate(4, 0, Time.deltaTime);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            if (objPool.Count > 0)
            {
                a2 = objPool.Dequeue();
                a2.SetActive(true);
            }
            else
            {
                a2 = Instantiate(gobj2);
            }
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            a
}
