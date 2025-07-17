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





Sure, I can help with that! Here is an updated version of the code that uses an object pool to instantiate the line segments:

    void Update()
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
        LineRenderer line = GetLineFromPool();
        line.positionCount = 1;
        line.SetPosition(0, inkPos);
        strokeWidth = line.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
    }

    LineRenderer GetLineFromPool()
    {
        LineRenderer line = null;
        foreach (Transform child in lineContainer.transform)
        {
            if (child.gameObject.activeSelf)
            {
                line = child.gameObject.GetComponent<LineRenderer>();
                lineContainer.transform.GetChild(0).gameObject.SetActive(false);
                break;
            }
        }
        if (line == null)
        {
            line = Instantiate(lineSegmentPrefab, lineContainer.transform).GetComponent<LineRenderer>();
        }
        return line;
    }

    void UpdateLine(Vector3 tipPosition)
    {
        if (inkPositions.Count == 0)
        {
            inkPositions.Add(tipPosition);
        }
        else
        {
            inkPositions.Add(tipPosition);
            LineRenderer line = GetLineFromPool();
            line.positionCount = inkPositions.Count;
            line.SetPosition(0, inkPositions[0]);
            for (int i = 1; i < inkPositions.Count; i++)
            {
                line.SetPosition(i, inkPositions[i]);
            }
            float dist = Vector3.Distance(inkPositions[inkPositions.Count - 2], inkPositions[inkPositions.Count - 1]);
            if (dist < minInkDist)
            {
                inkPositions.RemoveAt(inkPositions.Count - 1);
            }
            else
            {
                strokeLength += dist;
            }
        }
    }

This version of the code creates a new line renderer object from the object pool whenever a new line needs to be drawn, rather than instantiating a new object every time Update() is called. This can help improve performance and reduce memory usage. Additionally, the code adds a minimum distance requirement for line segments to prevent very short lines from being drawn.



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
