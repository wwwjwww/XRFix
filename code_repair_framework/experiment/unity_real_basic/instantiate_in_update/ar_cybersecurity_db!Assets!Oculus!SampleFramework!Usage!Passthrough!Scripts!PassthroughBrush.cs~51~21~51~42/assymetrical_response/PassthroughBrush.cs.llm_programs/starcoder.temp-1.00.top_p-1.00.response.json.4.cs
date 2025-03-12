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


/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     void LateUpdate()
*     {
* 
*         transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
* 
*         if (!(controllerHand == OVRInput.Controller.LTouch || controllerHand == OVRInput.Controller.RTouch))
*         {
*             return;
*         }
* 
*         Vector3 tipPosition = transform.position;
*         switch (brushStatus)
*         {
*             case BrushState.Idle:
*                 if (OVRInput.GetUp(OVRInput.Button.One, controllerHand))
*                 {
*                     UndoInkLine();
*                 }
* 
*                 if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
*                 {
*                     StartLine(tipPosition);
*                     brushStatus = BrushState.Inking;
*                 }
* 
*                 break;
*             case BrushState.Inking:
* 
*                 UpdateLine(tipPosition);
*                 if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
*                 {
*                     brushStatus = BrushState.Idle;
*                 }
* 
*                 break;
*         }
*     }





    private void Awake()
    {
        if (lineSegmentPrefab == null)
        {
            lineSegmentPrefab = Resources.Load<GameObject>("Prefabs/LineSegment");
        }
        if (lineContainer == null)
        {
            lineContainer = new GameObject("LineContainer");
            lineContainer.transform.parent = transform;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        if (!(controllerHand == OVRInput.Controller.LTouch || controllerHand == OVRInput.Controller.RTouch))
        {
            return;
        }

        Vector3 tipPosition = transform.position;
        switch (brushStatus)
        {
            case BrushState.Idle:
                if (OVRInput.GetUp(OVRInput.Button.One, controllerHand))
                {
                    UndoInkLine();
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
                {
                    StartLine(tipPosition);
                    brushStatus = BrushState.Inking;
                }

                break;
            case BrushState.Inking:

                UpdateLine(tipPosition);
                if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controllerHand))
                {
                    brushStatus = BrushState.Idle;
                }

                break;
        }
    }

    void StartLine(Vector3 inkPos)
    {
        GameObject newLine = null;

        if (lineSegmentPrefab == null)
        {
            newLine = Instantiate(lineSegmentPrefab, inkPos, Quaternion.identity);
        }
        else
        {
            newLine = Instantiate(lineSegmentPrefab, transform);
        }

        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.parent = lineContainer.transform;
    }

    void UpdateLine(Vector3 tipPosition)
    {
        if (inkPositions.Count > 0)
        {
            LineRenderer line = currentLineSegment;
            line.startWidth = strokeWidth;
            line.endWidth = strokeWidth;
            line.SetPosition(1, tipPosition);

            if (inkPositions.Count > 1)
            {
                // Calculate the distance between the previous and current position
                float dist = Vector3.Distance(inkPositions[inkPositions.Count - 2], tipPosition);

                // Check if the distance is larger than the minimum distance required to add a new point
                if (dist > minInkDist)
                {
                    // Add the new position to the list
                    inkPositions.Add(tipPosition);

                    // Calculate the new length of the line
                    strokeLength = 0.0f;
                    for (int i = 1; i < inkPositions.Count; i++)
                    {
                        strokeLength += Vector3.Distance(inkPositions[i - 1], inkPositions[i]);
                    }
                }
            }
        }
    }

feel free to ask me for any help you need!



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
