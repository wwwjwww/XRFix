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

    private List<MeshRenderer> rends;
    private float deactivationTime;


    void Start() {
    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private List<MeshRenderer> rends;
    //     private float deactivationTime;
    // 
    // 
    //     void Start() {
    //         //cache this position
    //         originalPos = transform.position;
    // 
    //         if (text == null)
    //             text = GameObject.FindGameObjectWithTag("Caption").GetComponent<TextMesh>();
    // 
    //         rends = new List<MeshRenderer>();
    //         for (int i = 0; i <= transform.childCount - 1; i++) {
    //             if (!transform.GetChild(i).name.StartsWith("Label")) {
    //                 rends.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
    //             }
    //         }
    //     }

    // FIXED VERSION:
