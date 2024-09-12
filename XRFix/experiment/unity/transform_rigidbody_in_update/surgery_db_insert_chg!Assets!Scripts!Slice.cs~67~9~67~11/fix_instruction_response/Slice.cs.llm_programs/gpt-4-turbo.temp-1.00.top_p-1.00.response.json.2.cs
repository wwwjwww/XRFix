using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;

    protected Rigidbody rb2;

    protected GameObject gobj7;

    protected GameObject a7;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh = Instantiate(smr.sharedMesh);
        smr.sharedMesh = mesh;
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        rb2 = GetComponent<Rigidbody>(); // If the Rigidbody component exists on this GameObject.
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.name == "Scalpel point")
        {
            Debug.Log("cutting");
            RaycastHit hit;
            var point_transform = collider.transform;
            if (meshCollider.Raycast(new Ray(point_transform.position, point_transform.forward * .02f), out hit, 1))
            {
                Debug.DrawLine(point_transform.position, hit.point, Color.red, 1);
                ModifyMesh(hit);
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
        timer += Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                ModifyMesh(hit);
            }
        }
    }

    void FixedUpdate()
    {
        rb2.transform.Rotate(0, 40 * Time.fixedDeltaTime, 0); // Use Time.fixedDeltaTime for consistency in physics calculation

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a7 = Instantiate(gobj7);
            timer = 0f;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Dispose(a7);
            timer = 0f;
            instantiate_gobj = false;
        }
    }

    private void ModifyMesh(RaycastHit hit)
    {
        List<int> triangles = new List<int>(mesh.triangles);
        int startIndex = hit.triangleIndex * 3;
        if (startIndex < triangles.Count)
        {
            triangles.RemoveRange(startIndex, 3);
            mesh.triangles = triangles.ToArray();
            meshCollider.sharedMesh = null; // Workaround to force MeshCollider update
            meshCollider.sharedMesh = mesh;
        }
    }
}
