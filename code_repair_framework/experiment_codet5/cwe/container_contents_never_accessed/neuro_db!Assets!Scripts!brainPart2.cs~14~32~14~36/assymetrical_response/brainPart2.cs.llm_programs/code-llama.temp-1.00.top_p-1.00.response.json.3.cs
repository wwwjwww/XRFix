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
    Here is the corrected code:

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

    void Start()
    {
        originalPos = transform.position;
        text = GetComponentInChildren<TextMesh>();
        text.text = "GREEN";
        isThisRed = false;
    }

    void Update()
    {
        if(isThisRed)
        {
            if(!inTransition)
            {
                DOTween.To(() => transform.position, x => transform.position = x, this.originalPos, 5);
                text.text = "GREEN";
                isThisRed = false;
            }
        }
    }

    void OnMouseDown()
    {
        if(!isThisRed)
        {
            text.text = "RED";
            DOTween.To(() => transform.position, x => transform.position = x, new Vector3(0,0,0), 5);
            isThisRed = true;
            inTransition = true;
        }
    }
}

The original code had several issues and was missing a few important lines such as "using DG.Tweening;". Also, the "OnMouseDown" function needed to be implemented to call the "isThisRed" bool in order to update the text to "RED" and the original pos to "Vector3(0,0,0)" as well as reset the "isThisRed" variable set to false.
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
