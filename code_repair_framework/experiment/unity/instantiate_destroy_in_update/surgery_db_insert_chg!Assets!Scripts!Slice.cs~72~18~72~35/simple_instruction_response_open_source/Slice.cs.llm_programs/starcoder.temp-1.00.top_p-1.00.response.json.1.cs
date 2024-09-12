Here is the updated code that should avoid the errors:

using UnityEngine;

public class Slice : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;
    private Rigidbody rb;
    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    private GameObject gobj7;
    private GameObject a7;
    private GameObject gobj;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        var smr = GetComponent<SkinnedMeshRenderer>();
        mesh = smr.sharedMesh;
        meshCollider = GetComponent<MeshCollider>();
    }

    void Update()
    {
        rb.transform.Rotate(0, 40f * Time.deltaTime, 0);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a7 = Instantiate(gobj7);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            Dispose(a7);
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
                triangles.RemoveRange(startIndex
}