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




The `Instantiate()` function is generally expensive when it's called frequently in the `Update()` method. To optimize this, you should pre-build a number of instances of your line segment prefab and store them in a pool in the `Start()` method. Then, you can just enable/disable or manipulate these stored objects in the `Update()` method as necessary. This also tends to make your program more memory efficient since you only have to hold onto a reference to the objects as long as they are active, and you prevent the constant need to allocate memory for new objects over time.

However, this optimization only makes sense if the number of line segments you're creating is high. In your case, the number of line segments created might not be high enough to justify this optimization. 

Also, please note that the line segment prefab should have a poolable component attached to it. For Unity built-in pooling, you can use the `PooledObject` and `ObjectPooler` scripts from Unity Asset Store.

And please ensure you have a good understanding of how Unity `Pooling` works and refactor your code accordingly so that each `Update()` method call fetches a line segment from the pool that could be reused for next frame's brush stroke.

    void StartLine(Vector3 inkPos)
    {
        GameObject newLine = Instantiate(lineSegmentPrefab, inkPos, Quaternion.identity);
        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.parent = lineContainer.transform;
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
