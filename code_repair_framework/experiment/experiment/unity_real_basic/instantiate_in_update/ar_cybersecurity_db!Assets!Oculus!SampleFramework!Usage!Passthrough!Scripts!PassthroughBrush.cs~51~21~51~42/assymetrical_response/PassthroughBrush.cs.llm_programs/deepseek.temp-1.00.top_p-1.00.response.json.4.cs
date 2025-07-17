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





private ObjectPool objectPool;

void Start() {
    objectPool = new ObjectPool(lineSegmentPrefab, 10); // Adjust the pool size as needed
}

void Update() {
    // Rest of your code...
}

void StartLine(Vector3 inkPos)
{
    // Get an inactive line segment from the object pool
    GameObject newLine = objectPool.GetObject();
    if (newLine != null) {
        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.parent = lineContainer.transform;
    } else {
        // If the object pool is empty, handle this situation as needed (e.g., log an error)
    }
}

public class ObjectPool {
    private GameObject prefab;
    private List<GameObject> pool;

    public ObjectPool(GameObject prefab, int size) {
        this.prefab = prefab;
        this.pool = new List<GameObject>();

        for (int i = 0; i < size; i++) {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObject() {
        foreach (GameObject obj in pool) {
            if (!obj.activeInHierarchy) {
                obj.SetActive(true);
                return obj;
            }
        }

        // No inactive objects left, so we instantiate a new one
        GameObject newObj = GameObject.Instantiate(prefab);
        pool.Add(newObj);
        return newObj;
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
