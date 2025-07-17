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





    Queue<GameObject> linePool = new Queue<GameObject>();

    int poolSize = 10; // Example pool size

    private void Start()
    {
        InitializeLinePool();
    }

    void LateUpdate()
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

    void InitializeLinePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject lineObj = Instantiate(lineSegmentPrefab);
            lineObj.SetActive(false);
            linePool.Enqueue(lineObj);
            lineObj.transform.parent = lineContainer.transform;
        }
    }

    GameObject GetPooledLine()
    {
        if (linePool.Count > 0)
        {
            GameObject lineObj = linePool.Dequeue();
            lineObj.SetActive(true);
            return lineObj;
        }
        else
        {
            // If the pool is empty, instantiate a new line (you might want to expand the pool instead)
            GameObject lineObj = Instantiate(lineSegmentPrefab);
            lineObj.transform.parent = lineContainer.transform;
            return lineObj;
        }
    }

    void ReleaseLine(GameObject lineObj)
    {
        lineObj.SetActive(false);
        linePool.Enqueue(lineObj);
    }

    void StartLine(Vector3 inkPos)
    {
        GameObject newLine = GetPooledLine(); 
        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.position = inkPos;
        newLine.transform.rotation = Quaternion.identity;
    }

    void UpdateLine(Vector3 newPos)
    {
        if (currentLineSegment == null) return;

        float dist = Vector3.Distance(inkPositions[inkPositions.Count - 1], newPos);
        if (dist > minInkDist)
        {
            inkPositions.Add(newPos);
            currentLineSegment.positionCount = inkPositions.Count;
            currentLineSegment.SetPosition(inkPositions.Count - 1, newPos);
            strokeLength += dist;
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
