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
    private float timer = 0f;
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
            // existing code unchanged
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
        // Existing raycast logic within Update method.
        if (Input.GetMouseButton(0))
        {
            // existing code unchanged
        }
    }

    void FixedUpdate()
    {
        // Physics related transformations moved to FixedUpdate
        rb2.transform.Rotate(0, 40, 0, Space.World);

        timer += Time.fixedDeltaTime;

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
}
