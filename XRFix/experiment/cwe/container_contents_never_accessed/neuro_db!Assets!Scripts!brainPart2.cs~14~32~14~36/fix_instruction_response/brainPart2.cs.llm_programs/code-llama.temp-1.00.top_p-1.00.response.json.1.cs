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

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private List<MeshRenderer> rends;

    //Remove or Commented-out the collection if it is no longer needed
    // FIXED CODE:
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

    // FIXED CODE:
    private List<MeshRenderer> rends; // Remove or Commented-out the collection if it is no longer needed

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    // private List<MeshRenderer> rends;

    void Start()
    {
        originalPos = transform.position;
        rends = new List<MeshRenderer>();
        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rends.Add(rend);
        }
    }

    void Update()
    {
        if (isThisRed)
        {
            if (inTransition)
            {
                float posY = Mathf.PingPong(Time.time, 0.5f) * 0.2f;
                transform.position = new Vector3(transform.position.x, posY, transform.position.z);
            }
            else
            {
                rends.ForEach(rend => rend.material.color = Color.red);
            }
        }
        else
        {
            rends.ForEach(rend => rend.material.color = Color.white);
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
