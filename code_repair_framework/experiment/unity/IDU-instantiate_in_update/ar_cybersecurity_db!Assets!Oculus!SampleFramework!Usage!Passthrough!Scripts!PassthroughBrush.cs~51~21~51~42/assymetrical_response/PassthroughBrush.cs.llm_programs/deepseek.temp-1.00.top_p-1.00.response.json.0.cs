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
    for (int i = 0; i < linePool.Length; i++)
    {
        linePool[i].SetActive(false);
    }
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





GameObject[] linePool;

int linePoolSize = 100; // adjust as needed

int currentLine;

private void Awake()
{
    linePool = new GameObject[linePoolSize];
    for (int i = 0; i < linePoolSize; i++)
    {
        linePool[i] = Instantiate(lineSegmentPrefab);
        linePool[i].SetActive(false);
        linePool[i].transform.parent = lineContainer.transform;
    }
}

void StartLine(Vector3 inkPos)
{
    currentLine = NextInactiveLine();
    if (currentLine == -1)
    {
        Debug.LogWarning("Line pool is full, cannot create new line.");
        return;
    }
    
    InitializeLine(inkPos);
}

void InitializeLine(Vector3 inkPos)
{
    linePool[currentLine].SetActive(true);
    currentLineSegment = linePool[currentLine].GetComponent<LineRenderer>();
    currentLineSegment.positionCount = 1;
    currentLineSegment.SetPosition(0, inkPos);
    strokeWidth = currentLineSegment.startWidth;
    strokeLength = 0.0f;
    inkPositions.Clear();
    inkPositions.Add(inkPos);
}

void AppendLine(Vector3 inkPos)
{
    currentLineSegment.positionCount++;
    currentLineSegment.SetPosition(currentLineSegment.positionCount - 1, inkPos);
    inkPositions.Add(inkPos);
}

int NextInactiveLine()
{
    for (int i = 0; i < linePool.Length; i++)
    {
        if (!linePool[i].activeInHierarchy)
        {
            return i;
        }
    }
    return -1; // no inactive lines left
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
