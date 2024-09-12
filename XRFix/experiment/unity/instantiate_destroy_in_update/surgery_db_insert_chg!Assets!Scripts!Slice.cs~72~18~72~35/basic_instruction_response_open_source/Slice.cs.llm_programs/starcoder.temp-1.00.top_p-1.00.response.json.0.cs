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

    protected GameObject gobj;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    // Start is called before the first frame update
    void Start()
    {
        var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh = smr.sharedMesh;
        meshCollider = gameObject.GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2.transform.Rotate(0, 40, 0);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a7 = Instantiate(gobj7, transform);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a7);
            timer = 0;
            instantiate_gobj = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.name == "Scalpel point")
        {
            RaycastHit hit;
            var point_transform = collider.transform;
            if (meshCollider.Raycast(new Ray(point_transform.position, point_transform.forward *.02f), out hit, 1))
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