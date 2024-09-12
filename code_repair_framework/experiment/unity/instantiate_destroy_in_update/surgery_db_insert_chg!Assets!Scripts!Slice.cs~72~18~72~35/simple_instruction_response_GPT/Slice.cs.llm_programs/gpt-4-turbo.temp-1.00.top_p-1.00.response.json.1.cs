//Here're the buggy code lines from /Assets/Scripts/Slice.cs:
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slice : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;

    protected Rigidbody rb2;

    protected GameObject gobj7;

    protected GameObject a7;

    protected GameObject gobj;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;



    void Start()
    {
        var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh = (Mesh)Instantiate(smr.sharedMesh);
        smr.sharedMesh = mesh;
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.name == "Scalpel point")
        {
            Debug.Log("cutting");
            RaycastHit hit;
            var point_transform = collider.transform;
            if (meshCollider.Raycast(new Ray(point_transform.position, point_transform.forward * .02f), out hit, 1)) {
                Debug.DrawLine(point_transform.position, hit.point, Color.red, 1);
                List<int> triangles = new List<int>();
                triangles.AddRange(mesh.triangles);
                int startIndex = hit.triangleIndex * 3;
                triangles.RemoveRange(startIndex, 3);
                mesh.triangles = triangles.ToArray();
                meshCollider.sharedMesh = mesh;
            }
        }
    }

    public void Dispose(GameObject obj7)
    {
        Destroy(obj7);
    }

    public void DisposeObj()
    {
        Destroy(gameObject);
    }



// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        rb2.transform.Rotate(0, 40, 0);
//
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit){
//            a7 = Instantiate(gobj7);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit ){
//            Dispose(a7);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        if (Input.GetMouseButton(0))
//        {
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//
//            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
//            {
//                
//                List<int> triangles = new List<int>();
//                triangles.AddRange(mesh.triangles);
//
//                
//                int startIndex = hit.triangleIndex * 3;
//
//                
//                
//                
//                triangles.RemoveRange(startIndex, 3);
//
//                
//                mesh.triangles = triangles.ToArray();
//                meshCollider.sharedMesh = mesh;
//            }
//        }
//    }
//    public void Dispose(GameObject obj7)
//    {
//        Destroy(obj7);
//    }

// FIXED CODE:

// FIXED CODE:

private const float SpawnDelay = 5f;
private GameObject spawnedObject;

void Awake() {
    InitializeTimers();
}

void Update()
{
    rb2.transform.Rotate(0, 40 * Time.deltaTime, 0);

    HandleObjectSpawning();
    HandleInput();
}

private void InitializeTimers() {
    if (!instantiate_gobj) {
        timer = timeLimit;
    } else {
        timer = 0f;
    }
}

private void HandleObjectSpawning() {
    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit) {
        spawnedObject = Instantiate(gobj7);
        instantiate_gobj = true;
        timer -= timeLimit;
    } else if (instantiate_gobj && timer >= timeLimit) {
        Dispose(spawnedObject);
        instantiate_gobj = false;
        timer -= timeLimit;
    }
}

private void HandleInput() {
    if (Input.GetMouseButton(0)) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask: Physics.DefaultRaycastLayers)) {
            if (hit.collider == meshCollider) {
                RemoveTriangleAtHit(hit);
            }
        }
    }
}

private void RemoveTriangleAtHit(RaycastHit hit) {
    List<int> triangles = new List<int>(mesh.triangles);
    int startIndex = hit.triangleIndex * 3;
    if (startIndex < triangles.Count - 3) { // ensuring there are enough triangles to remove
        triangles.RemoveRange(startIndex, 3);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); // recalculate normals to update lighting
        meshCollider.sharedMesh = null; // clear the existing sharedMesh
        meshCollider.sharedMesh = mesh; // reassign updated mesh to MeshCollider
    }
}

private void OnDisable() {
    Dispose(spawnedObject); // Clean up the instantiated object when this object is disabled
}

public void Dispose(GameObject obj7)
{
    if (obj7) {
        Destroy(obj7);
    }
}

}