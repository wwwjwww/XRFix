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

    private static List<GameObject> pool = new List<GameObject>();

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

    public static GameObject GetPooledObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            return obj;
        }
        else
        {
            return new GameObject();
        }
    }

    public static void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Add(obj);
    }

    void Update()
    {
        rb2.transform.Rotate(0, 40, 0);

        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit){
            a7 = GetPooledObject();
            a7.transform.position = transform.position;
            a7.transform.rotation = transform.rotation;
            a7.SetActive(true);
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit ){
            ReturnToPool(a7);
            instantiate_gobj = false;
        }

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (meshCollide
}