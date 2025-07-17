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





    private Stack<GameObject> lineSegmentPool = new Stack<GameObject>();

    private void Awake()
    {
        // Prepopulate the pool with line segments
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(lineSegmentPrefab);
            obj.SetActive(false);
            lineSegmentPool.Push(obj);
        }
    }

    private void LateUpdate()
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

    private void StartLine(Vector3 inkPos)
    {
        GameObject newLine = GetPooledLineSegment();
        newLine.transform.position = inkPos;
        newLine.transform.rotation = Quaternion.identity;
        newLine.SetActive(true);

        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.parent = lineContainer.transform;
    }

    private GameObject GetPooledLineSegment()
    {
        if (lineSegmentPool.Count > 0)
        {
            return lineSegmentPool.Pop();
        }
        else
        {
            // If the pool is empty, instantiate a new line segment
            GameObject obj = Instantiate(lineSegmentPrefab);
            return obj;
        }
    }

    private void ReturnLineSegmentToPool(GameObject lineSegment)
    {
        lineSegment.SetActive(false);
        lineSegmentPool.Push(lineSegment);
    }

    private void UpdateLine(Vector3 tipPosition)
    {
        if (currentLineSegment == null)
        {
            return;
        }

        if (inkPositions.Count > 0)
        {
            Vector3 lastPosition = inkPositions[inkPositions.Count - 1];
            if (Vector3.Distance(tipPosition, lastPosition) > minInkDist)
            {
                currentLineSegment.positionCount++;
                currentLineSegment.SetPosition(currentLineSegment.positionCount - 1, tipPosition);
                inkPositions.Add(tipPosition);
                strokeLength += Vector3.Distance(tipPosition, lastPosition);
            }
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
