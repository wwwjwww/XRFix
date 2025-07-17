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

    private void Awake()
    {
        originalPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (inTransition)
        {
            transform.position = Vector3.Lerp(transform.position, originalPos, Time.deltaTime * 5);

            if (transform.position == originalPos)
            {
                inTransition = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Red"))
        {
            isThisRed = true;
            transform.DOMove(originalPos, 1).OnComplete(() => inTransition = true);
        }
        else if (other.gameObject.CompareTag("Blue") && isThisRed == false)
        {
            transform.position = originalPos;
        }
    }

    public void Restart()
    {
        transform.position = originalPos;
        isThis
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
