Here's the corrected code:

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

    List<GameObject> obj_pool = new List<GameObject>();

    void Start()
    {
        var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
        mesh = (Mesh)Instantiate(smr.sharedMesh);
        smr.sharedMesh = mesh;
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        gobj7 = obj_pool.Find(o => o == null);
        if (gobj7 == null)
        {
            gobj7 = Instantiate(gobj7, Vector3.zero, Quaternion.identity);
            obj_pool.Add(gobj7);
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.name == "Scalpel point")
        {
            Debug.Log("cutting");
            RaycastHit hit;
            var point_transform = collider.transform;
            if (meshCollider.Raycast(new Ray(point_transform.position, point_transform.forward *.02f), out hit, 1))
            {
                Debug.DrawLine(point_transform.position, hit.point, Color.red, 1);
                List<int> triangles = new List<int>();
                triangles
}