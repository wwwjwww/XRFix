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



            *     }

            * Move this function in FixedUpdate() methods.
            * FIXED CODE:
            */


    void FixedUpdate()
    {
        if(Input.GetKey("up"))
        {
            speed += acceleration;
        }
        else if(Input.GetKey("down"))
        {
            speed -= deceleration;
        }
        
        if(Input.GetKey("space"))
        {
            speed += boost;
        }
        
        speed = Mathf.Clamp(speed, 0f, maxSpeed); // This is to ensure the speed doesn't exceed the maximum

        // Apply the speed to the rigidbody (or character controller, if you're using one)
        // You may need to replace Rigidbody with your actual type of Rigidbody used in your project.
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }



}
