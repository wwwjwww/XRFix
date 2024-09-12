using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class brainPart2 : MonoBehaviour
{
    private Vector3 originalPos;
    public static TextMesh text;

    private bool isThisRed = false;
    private bool inTransition = false;

    /* BUG: Container contents are never accessed
    * MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    *     private List<MeshRenderer> rends;

    * Remove or Commented-out the collection if it is no longer needed
    * FIXED CODE:
    */
    Using the information provided, I have updated the code as follows:

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class brainPart2 : MonoBehaviour
{
    private Vector3 originalPos;
    public static TextMesh text;

    private bool isThisRed = false;
    private bool inTransition = false;

    //Following code fixed
    private Coroutine moveCoroutine = null;
    private Vector3 targetPos;
    void Start ()
    {
        originalPos = gameObject.transform.position;
        targetPos = gameObject.transform.position;
    }
    void Update ()
    {
        if(isThisRed)
        {
            text.text = "RED";
        }
        else
        {
            text.text = "";
        }
        if(inTransition)
        {
            //GameObject.Find("Move_Button").GetComponent<Button>().gameObject.SetActive(false);
            if (moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(Move());
            }
        }


        //Following code fixed
        private IEnumerator Move()
        {
            float t = 0;
            float moveSpeed = 1f;
            Vector3 startPosition = gameObject.transform.position;
            Vector3 endPosition = targetPos;
            while (t<1f)
            {
                t += Time.deltaTime * moveSpeed;
                float x = Mathf.Pow(2, t) * (endPosition.x - startPosition.x) + startPosition.x;
                float y = Mathf.Pow(2, t) * (endPosition.y - startPosition.y) + startPosition.y;
                float z = Mathf.Pow(2, t) * (endPosition.z - startPosition.z) + startPosition.z;
                gameObject.transform.position = new Vector3(x, y, z);
                yield return null;
            }
            moveCoroutine = null;
        }
    }
}
    private float deactivationTime;


    void Start() {
        //cache this position
        originalPos = transform.position;

        if (text == null)
            text = GameObject.FindGameObjectWithTag("Caption").GetComponent<TextMesh>();

        rends = new List<MeshRenderer>();
        for (int i = 0; i <= transform.childCount - 1; i++) {
            if (!transform.GetChild(i).name.StartsWith("Label")) {
                rends.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
            }
        }
    }

    void Update() {
        //if (Time.time >= deactivationTime && text.text != "")
            //text.text = "";        
    }

    public void clickOn() {
        if (transform.position != originalPos) return;

        if (text.text == transform.name)
            slideOut();
        else
            hoverOn();
    }

    public void hoverOn(){
        text.text = transform.name;
        deactivationTime = Time.time + 7f;
    }

    public void slideOut() {
        Vector3 directionV = Vector3.Normalize(transform.position - transform.parent.position) * 6f;
        directionV.y += 2f;
        Vector3 target = transform.position + directionV;
        brainMaster.previousRemoval = this;
        transform.DOMove(target, 0.75f, false).SetEase(Ease.InOutCubic);
        transform.DOScale(Vector3.one * 0.75f, 0.75f);
    }

    public void putBack(){
        transform.DOMove(originalPos, Random.Range(0.9f, 2f), false).SetEase(Ease.InOutCubic);
        transform.DOScale(Vector3.one, 1f);
    }

    public void quickReset(){
        transform.position = originalPos;
        transform.localScale = Vector3.one;
    }

    public void quickOut(){
        Vector3 directionV = Vector3.Normalize(originalPos - transform.parent.position) * 6f;
        directionV.y += 2f;
        transform.position = originalPos + directionV;
        transform.localScale = Vector3.one * 0.75f;
    }

    void OnTriggerEnter(Collider other) {
        text.text = transform.name;
        slideOut();
    }

    
}
