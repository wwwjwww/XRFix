using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject cameraRig;
    public GameObject bike;


    public float speed = 0f;
    public float maxSpeed = 50f;

    public float acceleration = 0.05f;  // Triggered every second 
    public float deceleration = 0.5f;   // Triggered upon jump
    public float boost = 0.25f;         // Triggered upon enemyHit  

    public bool reachedFinishLine = false;

    public int score = 0;    //Score for each enemy hit


    void Start()
    {
        cameraRig = GameObject.Find("CameraRig");   //Import CameraRig (Parent of the VR Camera)
        bike = GameObject.Find("Bike");  //Import the bike
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
            //         
            //     }

            // FIXED CODE:


I've fixed the code you provided. Here's the updated code under the `FIXED CODE:` area:

    void Update()
    {
        
        float horizontalSpeed;
        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            // If no input, use acceleration to move
            horizontalSpeed = speed + acceleration * Time.deltaTime;
        }
        else
        {
            // If input, use deceleration
            horizontalSpeed = speed - deceleration * Time.deltaTime;
        }
        
        if (horizontalSpeed < 0)
        {
            horizontalSpeed = 0;
        }
        else if (horizontalSpeed > maxSpeed)
        {
            horizontalSpeed = maxSpeed;
        }

        // Update player's horizontal position
        transform.position += new Vector3(horizontalSpeed, 0, 0) * Time.deltaTime;

        // Update player's position based on cameraRig's position
        Vector3 cameraRigPosition = cameraRig.transform.position;
        transform.position = new Vector3(transform.position.x, cameraRigPosition.y, transform.position.z);

        // Update cameraRig's position based on player's position
        cameraRig.transform.position = new Vector3(transform.position.x, cameraRigPosition.y, transform.position.z);

        // Update gameObject's position based on cameraRig's position and bike's position
        Vector3 direction = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 newBikePosition = bike.transform.position + direction * speed * Time.deltaTime;
        Vector3 cameraRigPosition2 = cameraRig.transform.position;
        newBikePosition = new Vector3(Mathf.Clamp(newBikePosition.x, -4.5f, 4.5f), cameraRigPosition2.y, Mathf.Clamp(newBikePosition.z, -12f, 2f));
        bike.transform.position = newBikePosition;
    }



}
