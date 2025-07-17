using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject cameraRig;
    public GameObject bike;


    public float speed = 0f;
    public float maxSpeed = 50f;

    public float acceleration = 0.05f;  
    public float deceleration = 0.5f;   
    public float boost = 0.25f;         

    public bool reachedFinishLine = false;

    public int score = 0;    


    void Start()
    {
        cameraRig = GameObject.Find("CameraRig");   
        bike = GameObject.Find("Bike");  
    }


///     void Update()
//     {
// 
//         speed += acceleration * Time.deltaTime;
//         if (speed > maxSpeed) {
//             speed = maxSpeed;   // Setting a max speed 
//         }
// 
// 
//         Vector3 direction = new Vector3( transform.forward.x, 0,  transform.forward.z); 
//         
// 
//         cameraRig.transform.position +=  direction.normalized * speed *  Time.deltaTime;
// 
// 
// 
//         if (transform.position.x <-4.5f) {  //Left side 
//             transform.position = new Vector3(-4.5f, transform.position.y, transform.position.z); 
//         } else if (transform.position.x > 4.5f) {   // Right side
//             transform.position = new Vector3(4.5f, transform.position.y, transform.position.z);
//         }
// 
// 
// 
//         if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("space"))   {
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             bike.GetComponent<Rigidbody>().AddForce(transform.up * 1000f);
            //             speed -= deceleration;  // Decelerate upon each jump
            //         }
            // 
            // 
            // 
            //         if (bike.transform.position.x < -4.5f) {   // Left Edge
            //             bike.transform.position = new Vector3(bike.transform.position.x + 2f, bike.transform.position.y + 2, transform.position.z + 4.5f);
            //         } else if (bike.transform.position.x > 4.5f) {   // Right Edge
            //             bike.transform.position = new Vector3(bike.transform.position.x - 2f, bike.transform.position.y + 2, transform.position.z + 4.5f);
            //         }
            // 
            // 
            // 
            //         if (bike.transform.position.z > this.transform.position.z + 12f) {  // If infront of VR Camera
            //             bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 11.9f);
            //         } else  if (bike.transform.position.z < this.transform.position.z - 2f) {  // If behind the VR Camera
            //             bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 1f);
            //         }
            // 
            // 
            // 
            //         /*
            // 
            //         RaycastHit hit;
            //         if (Physics.Raycast(transform.position, transform.forward, out hit)) {
            //              Debug.Log(hit.transform.name);  
            //          }
            //         */
            //     }

            // FIXED CODE:


    void FixedUpdate()
    {
        speed += acceleration * Time.deltaTime;
        if (speed > maxSpeed) 
        {
            speed = maxSpeed;   
        }

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("space"))   
        {
            bike.GetComponent<Rigidbody>().AddForce(transform.up * 1000f);
            speed -= deceleration;  
        }
        
        if (bike.transform.position.x < -4.5f) 
        {   
            bike.transform.position = new Vector3(-4.5f, transform.position.y, bike.transform.position.z); 
        } 
        else if (bike.transform.position.x > 4.5f) 
        {   
            bike.transform.position = new Vector3(4.5f, transform.position.y, bike.transform.position.z);
        }
        
        if (bike.transform.position.z > this.transform.position.z + 12f) 
        {  
            bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 11.9f);
        } 
        else  if (bike.transform.position.z < this.transform.position.z - 2f) 
        {  
            bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 1f);
        }
    }



}
