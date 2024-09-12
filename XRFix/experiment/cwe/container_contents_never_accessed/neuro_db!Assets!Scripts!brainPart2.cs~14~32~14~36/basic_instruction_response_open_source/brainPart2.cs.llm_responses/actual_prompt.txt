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

    // FIXED CODE:
