using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject cameraRig;
    public GameObject bike;

    //public float bikedistance = 2f;
    public float speed = 0f;
    public float maxSpeed = 50f;

    public float acceleration = 0.05f;  // Triggered every second 
    public float deceleration = 0.5f;   // Triggered upon jump
    public float boost = 0.25f;         // Triggered upon enemyHit  

    public bool reachedFinishLine = false;

    public int score = 0;    //Score for each enemy hit

    // Start is called before the first frame update
    void Start()
    {
        cameraRig = GameObject.Find("CameraRig");   //Import CameraRig (Parent of the VR Camera)
        bike = GameObject.Find("Bike");  //Import the bike 
    }

    // Update is called once per frame
    void Update()
    {
        //  Acceleration Logic
        speed += acceleration * Time.deltaTime;
        if (speed > maxSpeed) {
            speed = maxSpeed;   // Setting a max speed 
        }

        // Directional Vector to only let the player move horizontally
        Vector3 direction = new Vector3( transform.forward.x, 0,  transform.forward.z); 
        
        // Move the player (VR Camera) to where you move your head or mouse
        cameraRig.transform.position +=  direction.normalized * speed *  Time.deltaTime;


        // Make player (VR Camera) stay inside the road 
        if (transform.position.x <-4.5f) {  //Left side 
            transform.position = new Vector3(-4.5f, transform.position.y, transform.position.z); 
        } else if (transform.position.x > 4.5f) {   // Right side
            transform.position = new Vector3(4.5f, transform.position.y, transform.position.z);
        }


        // Make bike jump 
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("space"))   {
            bike.GetComponent<Rigidbody>().AddForce(transform.up * 1000f);
            speed -= deceleration;  // Decelerate upon each jump
        }


        // Make bike stay inside the road
        if (bike.transform.position.x < -4.5f) {   // Left Edge
            bike.transform.position = new Vector3(bike.transform.position.x + 2f, bike.transform.position.y + 2, transform.position.z + 4.5f);
        } else if (bike.transform.position.x > 4.5f) {   // Right Edge
            bike.transform.position = new Vector3(bike.transform.position.x - 2f, bike.transform.position.y + 2, transform.position.z + 4.5f);
        }


        //Stop the bike from going too far infront or behind of the VR camera
        if (bike.transform.position.z > this.transform.position.z + 12f) {  // If infront of VR Camera
            bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 11.9f);
        } else  if (bike.transform.position.z < this.transform.position.z - 2f) {  // If behind the VR Camera
            bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 1f);
        }



        /*
        // Test if GoogleVR is working by sending out Raycasts 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit)) {
             Debug.Log(hit.transform.name);  
         }
        */
    }

}
