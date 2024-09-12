using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///     void Update()
//     {
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb2.transform.Rotate(0, 40, 0);
        // 
        //         timer+=Time.deltaTime;
        // 
        //         if (!instantiate_gobj && timer >= timeLimit){
        //             a7 = Instantiate(gobj7);
        //             timer = 0;
        //             instantiate_gobj = true;
        //         }
        //         if (instantiate_gobj && timer >= timeLimit ){
        //             Dispose(a7);
        //             timer = 0;
        //             instantiate_gobj = false;
        //         }
        // 
        //         if (Input.GetMouseButton(0))
        //         {
        //             RaycastHit hit;
        //             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 
        //             if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
        //             {
        //                 
        //                 List<int> triangles = new List<int>();
        //                 triangles.AddRange(mesh.triangles);
        // 
        //                 
        //                 int startIndex = hit.triangleIndex * 3;
        // 
        //                 
        //                 
        //                 
        //                 triangles.RemoveRange(startIndex, 3);
        // 
        //                 
        //                 mesh.triangles = triangles.ToArray();
        //                 meshCollider.sharedMesh = mesh;
        //             }
        //         }
        //     }

        // FIXED CODE:
