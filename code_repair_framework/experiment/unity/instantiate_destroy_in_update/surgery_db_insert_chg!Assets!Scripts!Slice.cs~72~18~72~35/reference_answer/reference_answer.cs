private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj7);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   rb1.transform.Rotate(30, 0, 0);

   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a7 = objectPool.Dequeue();
        a7.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a7.SetActive(false);
        objectPool.Enqueue(a7);
        timer = 0;
        instantiate_gobj = false;
   }
   if (Input.GetMouseButton(0))
   {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
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