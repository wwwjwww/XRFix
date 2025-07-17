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





    void LateUpdate()
    {
        // Get the current controller hand from the OVRInput API
        OVRInput.Controller currentControllerHand = OVRInput.GetConnectedController();

        // If the controller hand is None, early out of the function
        if (currentControllerHand == OVRInput.Controller.None) {
            return;
        }

        // Get the tip position from the OVRInput API
        Vector3 tipPosition = OVRInput.GetLocalControllerPosition(currentControllerHand);

        // Switch on the current brush status
        switch (brushStatus) {
            case BrushState.Idle:
                // If the button is pressed, undo the ink line
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, currentControllerHand)) {
                    UndoInkLine();
                }

                // If the button is pressed, start the ink line
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, currentControllerHand)) {
                    StartLine(tipPosition);
                    brushStatus = BrushState.Inking;
                }
                break;
            case BrushState.Inking:
                // If the button is released, end the ink line
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, currentControllerHand)) {
                    brushStatus = BrushState.Idle;
                }
                break;
        }
    }

    void StartLine(Vector3 inkPos)
    {
        // Instantiate the line segment prefab, set its start width to the desired value
        GameObject newLine = Instantiate(lineSegmentPrefab, inkPos, Quaternion.identity);
        newLine.GetComponent<LineRenderer>().startWidth = strokeWidth;

        // Set the line segment parent to the line container object
        newLine.transform.parent = lineContainer.transform;

        // Get the line segment component
        LineRenderer line = newLine.GetComponent<LineRenderer>();

        // Set the line segment position count to 1
        line.positionCount = 1;

        // Set the line segment position at index 0 to the current ink position
        line.SetPosition(0, inkPos);

        // Clear the list of ink positions
        inkPositions.Clear();

        // Add the current ink position to the list of ink positions
        inkPositions.Add(inkPos);

        // Set the current line segment to the instantiated line segment
        currentLineSegment = line;
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
