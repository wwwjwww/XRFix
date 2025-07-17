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
    private Transform transform;
 private bool isThisRed = false;
 private bool inTransition = false;

public void Start()
 {
    transform = GetComponent<Transform>();
    originalPos = transform.position;
    text = GameObject.Find("TextMesh").GetComponent<TextMesh>();
}

private void Update()
{
// BUG: Object is never changed and only ever used in one place, so there is no need to use a list to store it.
// MESSAGE: A local variable or a field whose value is set once and never modified is useless.
//    private List<MeshRenderer> rends;
    //Access the renderer directly
// BUG: The collection is never accessed if it is not used
// MESSAGE: A collection or map whose contents are never queried or accessed is useless.
//    private List<MeshRenderer> rends;

    if(input.GetMouseButtonDown(0))
    {
        // BUG: The original position is not modified, but its value is used in the if check
        // MESSAGE: A local variable or a field whose value is set once and never modified is useless.
        // originalPos = transform.position;

        if(transform.position == originalPos)
        {
            isThisRed = true;
            inTransition = true;
            transform.DOMove(transform.position, 1).SetRelative();
            text.text = "I'm red!";
        }
        else
        {
            isThisRed = false;
            inTransition = true;
            transform.DOMove(originalPos, 1).SetRelative();
            text.text = "I'm white!";
        }
    }

    if(inTransition)
    {
        // BUG: The condition is simply checking if the time has elapsed for the entire time it has been in the state.
        //  However, this is not what we want because the transition time elapsed will be the same throughout the entire transitio, even
        //  if the state has changed multiple times throughout that time.
        // MESSAGE: A local variable or a field whose value is set once and never modified is useless.
        // if (Time.time > 1.0)
        // {
        //   inTransition = false;
        // }
    }
}
private bool isThisRed = false;
private bool inTransition = false;

public void Start()
{
    transform = GetComponent<Transform>();
    originalPos = transform.position;
    text = GameObject.Find("TextMesh").GetComponent<TextMesh>();
}

private void Update()
{
// BUG: Object is never changed and only ever used in one place, so there is no need to use a list to store it.
// MESSAGE: A local variable or a field whose value is set once and never modified is useless.
//    private List<MeshRenderer> rends;
    //Access the renderer directly
// BUG: The collection is never accessed if it is not used
// MESSAGE: A collection or map whose contents are never queried or accessed is useless.
//    private List<MeshRenderer> rends;

    if(input.GetMouseButtonDown(0))
    {
        // BUG: The original position is not modified, but its value is used in the if check
        // MESSAGE: A local variable or a field whose value is set once and never modified is useless.
        // originalPos = transform.position;

        if(transform.position == originalPos)
        {
            isThisRed = true;
            inTransition = true;
            transform.DOMove(transform.position, 1).SetRelative();
            text.text = "I'm red!";
        }
        else
        {
            isThisRed = false;
            inTransition = true;
            transform.DOMove(originalPos, 1).SetRelative();
            text.text = "I'm white!";
        }
    }

    if(inTransition)
    {
        // BUG: The condition is simply checking if the time has elapsed for the entire time it has been in the state.
        //  However, this is not what we want because the transition time elapsed will be the same throughout the entire transitio, even
        //  if the state has changed multiple times throughout that time.
        // MESSAGE: A local variable or a field whose value is set once and never modified is useless.
        // if (Time.time > 1.0)
        // {
        //   inTransition = false;
        // }
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
