using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughBrush : MonoBehaviour
{
    public OVRInput.Controller controllerHand = OVRInput.Controller.None;
    public GameObject lineSegmentPrefab;
    public GameObject lineContainer;
    public bool forceActive = true;
    LineRenderer currentLineSegment;
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
        // Reset the brush status
        brushStatus = BrushState.Idle;
    }


// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
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





    List<LineRenderer> lineSegmentPool = new List<LineRenderer>();

    public Transform lineContainer;

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        if (!(OVRInput.Get(OVRInput.Button.LTouch) || OVRInput.Get(OVRInput.Button.RTouch)))
        {
            return;
        }

        Vector3 tipPosition = transform.position;

        // Check if there is an available line segment in the pool, and if so, retrieve it
        if (lineSegmentPool.Count > 0)
        {
            currentLineSegment = lineSegmentPool[0];
            lineSegmentPool.RemoveAt(0);
        }
        // Otherwise, instantiate a new line segment
        else
        {
            currentLineSegment = Instantiate(lineSegmentPrefab, lineContainer).GetComponent<LineRenderer>();
        }

        switch (brushStatus)
        {
            case BrushState.Idle:
                // If the index trigger is pressed and released, undo the last line segment
                if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch) || OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch))
                {
                    UndoInkLine();
                }

                // If the primary index trigger is pressed, start a new line segment
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    StartLine(tipPosition);
                    brushStatus = BrushState.Inking;
                }

                break;
            case BrushState.Inking:
                // Update the current line segment with the latest position
                UpdateLine(tipPosition);

                // If the primary index trigger is released, stop drawing
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
                {
                    brushStatus = BrushState.Idle;
                }

                break;
        }
    }

    void StartLine(Vector3 inkPos)
    {
        // Set the position of the line segment
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);

        // Save the starting thickness of the line
        strokeWidth = currentLineSegment.startWidth;

        // Reset the length of the line
        strokeLength = 0.0f;

        // Clear the list of positions
        inkPositions.Clear();

        // Add the first position to the list
        inkPositions.Add(inkPos);

        // Set the parent of the line segment to the line container
        currentLineSegment.transform.parent = lineContainer;
    }

        float segmentLength = 0.0f;

            float avgDist = segmentLength / (inkPositions.Count - 1);


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
