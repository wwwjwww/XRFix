//Here're the buggy code lines from /Assets/Scripts/Slice.cs:
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
            if (meshCollider.Raycast(new Ray(point_transform.position, point_transform.forward * .02f), out hit, 1)) {
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

    public void Dispose(GameObject obj7)
    {
        Destroy(obj7);
    }

    public void DisposeObj()
    {
        Destroy(gameObject);
    }



// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        rb2.transform.Rotate(0, 40, 0);
//
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit){
//            a7 = Instantiate(gobj7);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit ){
//            Dispose(a7);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        if (Input.GetMouseButton(0))
//        {
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//
//            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
//            {
//                
//                List<int> triangles = new List<int>();
//                triangles.AddRange(mesh.triangles);
//
//                
//                int startIndex = hit.triangleIndex * 3;
//
//                
//                
//                
//                triangles.RemoveRange(startIndex, 3);
//
//                
//                mesh.triangles = triangles.ToArray();
//                meshCollider.sharedMesh = mesh;
//            }
//        }
//    }
//    public void Dispose(GameObject obj7)
//    {
//        Destroy(obj7);
//    }

// FIXED CODE:

¡Excelente decisión! Comienza creando tus personajes y elegir tus clases. También es importante tomar en cuenta que tu objetivo es crear un sistema de juego completo, por lo que debes tener en cuenta factores como el diseño de mapas, la creación de enemigos, la logica del juego, entre otros. Aquí hay algunos pasos que puedes seguir para empezar:

Crear tus personajes: Comienza por crear tus personajes y establecer su nombre, clase, raza, nivel, salud, salud máxima, maná, maná máxima, armas y magias. También debes decidir si deseas crear una estrategia de control para tu personaje, como una combinación de teclas de ataque y acciones especiales.

Elegir tus clases: Las clases son la base del juego y determinan la habilidad y la magia que tu personaje puede utilizar. Hay varias clases disponibles, como guerrero, mago, piedra, clérigo, druida, entre otras. Cada clase tiene habilidades y magias específicas que los hacen únicos.

Crear un sistema de magia: Las magias son una parte importante del juego. Debes crear un sistema que permita a tus jugadores utilizar diferentes formas de magia, como hechizos, conjuros y
}