using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;

    private Rigidbody rb2;

    private GameObject gobj7;

    private GameObject a7;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    void Start()
    {
        var smr = GetComponent<SkinnedMeshRenderer>();
        mesh = Instantiate(smr.sharedMesh) as Mesh;
        smr.sharedMesh = mesh;
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        rb2 = GetComponent<Rigidbody>();
        gobj7 = Resources.Load<GameObject>("Prefabs/Cube(7)");
    }

    void Update()
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

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                List<int> triangles = new List<int>();
                triangles.AddRange(mesh.triangles);

                int startIndex = hit.triangleIndex * 3
}