/* Here're the buggy code lines from /Assets/Scripts/Slice.cs:*/
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



/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     void Update()
*     {
*         rb2.transform.Rotate(0, 40, 0);
* 
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit){
*             a7 = Instantiate(gobj7);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit ){
*             Dispose(a7);
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
*         if (Input.GetMouseButton(0))
*         {
*             RaycastHit hit;
*             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
* 
*             if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
*             {
*                 
*                 List<int> triangles = new List<int>();
*                 triangles.AddRange(mesh.triangles);
* 
*                 
*                 int startIndex = hit.triangleIndex * 3;
* 
*                 
*                 
*                 
*                 triangles.RemoveRange(startIndex, 3);
* 
*                 
*                 mesh.triangles = triangles.ToArray();
*                 meshCollider.sharedMesh = mesh;
*             }
*         }
*     }
*     public void Dispose(GameObject obj7)
*     {
*         Destroy(obj7);
*     }

* you can try to build an object pool before Update() method has been called.
* FIXED CODE:
*/

/* FIXED CODE: */
private GameObject pooledObject;
private bool isObjectPooled = false;

void Awake()
{
    pooledObject = Instantiate(gobj7);
    pooledObject.SetActive(false);
    isObjectPooled = true;
}

void Update()
{
    rb2.transform.Rotate(0, 40, 0);

    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit && isObjectPooled)
    {
        pooledObject.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit && isObjectPooled)
    {
        pooledObject.SetActive(false);
        timer = 0;
        instantiate_gobj = false;
    }

    if (Input.GetMouseButton(0))
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
        {
            List<int> triangles = new List<int>(mesh.triangles);
            int startIndex = hit.triangleIndex * 3;
            triangles.RemoveRange(startIndex, 3);
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            meshCollider.sharedMesh = mesh;
        }
    }
}

}