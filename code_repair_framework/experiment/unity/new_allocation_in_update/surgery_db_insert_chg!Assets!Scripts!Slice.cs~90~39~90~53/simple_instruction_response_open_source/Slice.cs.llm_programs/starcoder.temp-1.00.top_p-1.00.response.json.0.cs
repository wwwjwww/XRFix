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



    void Update()
    {
        rb2.transform.Rotate(0, 40, 0);

        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit){
            a7 = Instantiate(gobj7);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit ){
            Dispose(a7);
            timer = 0;
            instantiate_gobj = false;
        }

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                
                
                
The code should work, but you probably meant to instantiate a prefab instead of creating an object from the code.

Also, there are some unnecessary variables like "a7" and "gobj7" that you could remove.

Here's how the code could look:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;
    private Rigidbody rb;

    public GameObject prefab;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiatePrefab = false;

    void Start()
    {
        var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh = smr.sharedMesh;
        meshCollider = GetComponent<MeshCollider>();
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.name == "Scalpel point")
        {
            Debug.Log("cutting");
            RaycastHit hit;
            var point_transform = collider.transform;
            if (meshCollider.Raycast(new Ray(point_transform.position, point_transform.forward *.02f), out hit, 1)) {
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

    void Update()
    {
        rb.transform.Rotate(0, 40, 0);

        timer+=Time.deltaTime;

        if (!instantiatePrefab && timer >= timeLimit){
            Instantiate(prefab, transform.position, Quaternion.identity);
            timer = 0;
            instantiatePrefab = true;
        }
        if (instantiatePrefab && timer >= timeLimit ){
            Destroy(gameObject);
            timer = 0;
            instantiatePrefab = false;
        }

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log("Clicked!");
            }
        }
    }
}
                triangles.AddRange(mesh.triangles);

                // Calculate the startIndex (At what number we start removing)
                int startIndex = hit.triangleIndex * 3;

                // RemoveRange first parameter is index (at what number we start removing),
                // Which is our earlier calculated startIndex.
                // We want to delete 3 vertices, which is the second parameter here
                triangles.RemoveRange(startIndex, 3);

                // Update the triangles, we must convert our List to an Array here
                mesh.triangles = triangles.ToArray();
                meshCollider.sharedMesh = mesh;
            }
        }
    }
}