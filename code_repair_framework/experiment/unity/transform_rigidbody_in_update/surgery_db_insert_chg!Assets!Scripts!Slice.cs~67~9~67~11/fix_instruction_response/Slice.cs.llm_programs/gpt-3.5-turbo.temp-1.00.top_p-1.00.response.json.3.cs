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
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb2.transform.Rotate(0, 40, 0);
        // 
        //         timer+=Time.deltaTime;
        // 
        //         if (!instantiate_gobj && timer >= timeLimit){
        //             a7 = Instantiate(gobj7);
        //             timer = 0;
        //             instantiate_gobj = true;
        //         }
        //         if (instantiate_gobj && timer >= timeLimit ){
        //             Dispose(a7);
        //             timer = 0;
        //             instantiate_gobj = false;
        //         }
        // 
        //         if (Input.GetMouseButton(0))
        //         {
        //             RaycastHit hit;
        //             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 
        //             if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
        //             {
        //                 
        //                 List<int> triangles = new List<int>();
        //                 triangles.AddRange(mesh.triangles);
        // 
        //                 
        //                 int startIndex = hit.triangleIndex * 3;
        // 
        //                 
        //                 
        //                 
        //                 triangles.RemoveRange(startIndex, 3);
        // 
        //                 
        //                 mesh.triangles = triangles.ToArray();
        //                 meshCollider.sharedMesh = mesh;
        //             }
        //         }
        //     }

        // Move this function in FixedUpdate() methods.
        // FIXED CODE:

        }
void FixedUpdate()
        {
            rb2.transform.Rotate(0, 40, 0);

            timer += Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                a7 = Instantiate(gobj7);
                timer = 0;
                instantiate_gobj = true;
            }
            if (instantiate_gobj && timer >= timeLimit)
            {
                Dispose(a7);
                timer = 0;
                instantiate_gobj = false;
            }
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
                {

                    List<int> triangles = new List<int>();
                    triangles.AddRange(mesh.triangles);

                    int startIndex = hit.triangleIndex * 3;

                    triangles.RemoveRange(startIndex, 3);

                    mesh.triangles = triangles.ToArray();
                    meshCollider.sharedMesh = mesh;
                }
            }
        }

}