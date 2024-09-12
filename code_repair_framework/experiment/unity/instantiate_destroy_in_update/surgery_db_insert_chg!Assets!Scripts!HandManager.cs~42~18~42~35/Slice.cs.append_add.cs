


    void Update()
    {
        rb2.transform.Rotate(0, 40, 0);

        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit){
            a7 = Instantiate(gobj7);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit ){
            Dispose(a7);
            timer = 0;
            instantiate_gobj = false;
        }

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Store the triangles in a list
                List<int> triangles = new List<int>();
                triangles.AddRange(mesh.triangles);

                // Calculate the startIndex (At what number we start removing)
                int startIndex = hit.triangleIndex * 3;

                // RemoveRange first parameter is index (at what number we start removing),
                // Which is our earlier calculated startIndex.
                // We want to delete 3 vertices, which is the second parameter here
                triangles.RemoveRange(startIndex, 3);

                // Update the triangles, we must convert our List to an Array here
                mesh.triangles = triangles.ToArray();
                meshCollider.sharedMesh = mesh;
            }
        }
    }
}