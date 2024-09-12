c#
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

    private GameObject objPool;

    // Start is called before the first frame update
    void Start()
    {
        var smr = GetComponent<SkinnedMeshRenderer>();
        mesh = smr.sharedMesh;
        meshCollider = GetComponent<MeshCollider>();

        rb2 = GetComponent<Rigidbody>();
        gobj7 = Instantiate(Resources.Load("Prefab/obj7") as GameObject, transform.position, transform.rotation);
        a7 = null;
        gobj = Instantiate(Resources.Load("Prefab/obj") as GameObject, transform.position, transform.rotation);

        objPool = new GameObject("Object Pool");
    }

    // Update is called once per frame
    void Update()
    {
        rb2.transform.Rotate(Vector3.up * 40 * Time.deltaTime);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a7 = objPool.transform.Find("obj7").gameObject;
            if (a7 == null)
            {
                a7 = Instantiate(gobj7, objPool.transform);
            }
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
                List<int> triangles = new List<int>(mesh.triangles);

                int startIndex = hit.triangleIndex * 3;
                triangles.RemoveRange(startIndex, 3);

                mesh.triangles = triangles.ToArray();
                meshCollider.sharedMesh = mesh;
            }
        }
    }

    public void Dispose(GameObject obj7)
    {
        obj7.transform.SetParent(objPool.transform);
    }
}
}