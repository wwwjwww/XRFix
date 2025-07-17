using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughBrush : MonoBehaviour
{
    public OVRInput.Controller controllerHand = OVRInput.Controller.None;
    public GameObject lineSegmentPrefab;
    public GameObject lineContainer;
    public bool forceActive = true;
    LineRenderer currentLineSegment = null;
    List<Vector3> inkPositions = new List<Vector3>();
    float minInkDist = 0.01f;
    float strokeWidth = 0.1f;
    float strokeLength = 0.0f;

    public enum BrushState
    {
        Idle,
        Inking
    };

    BrushState brushStatus = BrushState.Idle;

    private void OnDisable()
    {
        brushStatus = BrushState.Idle;
    }


//    void LateUpdate()
//    {
//
//        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
//
//        if (!(controllerHand == OVRInput.Controller.LTouch || controllerHand == OVRInput.Controller.RTouch))
//        {
//            return;
//        }
//
//        Vector3 tipPosition = transform.position;
//        switch (brushStatus)
//        {
//            case BrushState.Idle:
//                if (OVRInput.GetUp(OVRInput.Button.One, controllerHand))
//                {
//                    UndoInkLine();
//                }
//
//                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
//                {
//                    StartLine(tipPosition);
//                    brushStatus = BrushState.Inking;
//                }
//
//                break;
//            case BrushState.Inking:
//
//                UpdateLine(tipPosition);
//                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
//                {
//                    brushStatus = BrushState.Idle;
//                }
//
//                break;
//        }
//    }





private void LateUpdate()
{
    // Get the current position and rotation of the controller
    Vector3 tipPosition = transform.position;
    Quaternion orientation = transform.rotation;

    // Rotate the tip position to be in the local space of the controller
    tipPosition = orientation * tipPosition;

    // Set the rotation of the line renderer to be the same as the controller
    currentLineSegment.transform.rotation = orientation;

    // Only render the line on the left or right controller
    if (controllerHand != OVRInput.Controller.LTouch && controllerHand != OVRInput.Controller.RTouch)
    {
        return;
    }

    // Update the line renderer's end position and width
    if (brushStatus == BrushState.Inking)
    {
        strokeLength += strokeWidth;
        currentLineSegment.SetPosition(1, tipPosition);
        currentLineSegment.widthMultiplier = strokeWidth;
    }

    // Start a new line if the user presses the primary index trigger
    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
    {
        brushStatus = BrushState.Inking;
        StartLine(tipPosition);
    }

    // End the line if the user releases the primary index trigger
    if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
    {
        brushStatus = BrushState.Idle;
    }

    // Check if the line has exceeded the maximum length and delete the last ink position if it has
    if (strokeLength > 0.02)
    {
        inkPositions.RemoveAt(inkPositions.Count - 1);
    }
}



    void UpdateLine(Vector3 inkPos)
    {
        float segmentLength = (inkPos - inkPositions[inkPositions.Count - 1]).magnitude;
        if (segmentLength >= minInkDist)
        {
            inkPositions.Add(inkPos);
            currentLineSegment.positionCount = inkPositions.Count;
            currentLineSegment.SetPositions(inkPositions.ToArray());
            strokeLength += segmentLength;
            // passing the line length to the shader ensures that the tail/end fades are consistent width
            currentLineSegment.material.SetFloat("_LineLength", strokeLength / strokeWidth);
        }
    }

    public void ClearLines()
    {
        for (int i = 0; i < lineContainer.transform.childCount; i++)
        {
            Destroy(lineContainer.transform.GetChild(i).gameObject);
        }
    }

    public void UndoInkLine()
    {
        if (lineContainer.transform.childCount >= 1)
        {
            Destroy(lineContainer.transform.GetChild(lineContainer.transform.childCount - 1).gameObject);
        }
    }
}
