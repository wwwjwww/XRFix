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
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh = (Mesh)Instantiate(smr.sharedMesh);
        smr.sharedMesh = mesh;
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        rb2 = GetComponent<Rigidbody>(); // Assuming there's a Rigidbody attached to the GameObject
    }

    void OnTriggerStay(Collider collider)
    {
        // ... existing OnTriggerStay code ...
    }

    public void Dispose(GameObject obj7)
    {
        Destroy(obj7);
    }

    public void DisposeObj()
    {
        Destroy(gameObject);
    }

    // Note that FixedUpdate is used for Rigidbody and physics related updates
    void FixedUpdate()
    {
        // Relocated code for Rigidbody rotation
        rb2.transform.Rotate(0, 40 * Time.fixedDeltaTime, 0);
    }

    void Update()
    {
        // Manage timed instantiation and disposal of objects
        timer += Time.deltaTime;
        if (!instantiate_gobj && timer >= timeLimit)
        {
            a7 = Instantiate(gobj7);
            timer = 0f;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            Dispose(a7);
            timer = 0f;
            instantiate_gobj = false;
        }

        // Handling raycast inside Update as it's input-related
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
                mesh.RecalculateNormals(); // Recalculate normals to resolve visual artifacts
                meshCollider.sharedMesh = mesh;
            }
        }
    }
}
