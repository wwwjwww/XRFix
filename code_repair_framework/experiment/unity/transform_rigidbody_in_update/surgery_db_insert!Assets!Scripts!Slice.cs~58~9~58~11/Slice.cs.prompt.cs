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



    void Update()
    {
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb2.transform.Rotate(0, 40, 0);
        // 
        //         GameObject a7 = Instantiate(gobj7);
        //         Dispose(a7);
        // 
        //         if (Input.GetMouseButton(0))
        //         {
        //             RaycastHit hit;
        //             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 
        //             if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
        //             {
        //                 // Store the triangles in a list
        //                 List<int> triangles = new List<int>();
        //                 triangles.AddRange(mesh.triangles);
        // 
        //                 // Calculate the startIndex (At what number we start removing)
        //                 int startIndex = hit.triangleIndex * 3;
        // 
        //                 // RemoveRange first parameter is index (at what number we start removing),
        //                 // Which is our earlier calculated startIndex.
        //                 // We want to delete 3 vertices, which is the second parameter here
        //                 triangles.RemoveRange(startIndex, 3);
        // 
        //                 // Update the triangles, we must convert our List to an Array here
        //                 mesh.triangles = triangles.ToArray();
        //                 meshCollider.sharedMesh = mesh;
        //             }
        //         }
        //     }

        // FIXED VERSION:
