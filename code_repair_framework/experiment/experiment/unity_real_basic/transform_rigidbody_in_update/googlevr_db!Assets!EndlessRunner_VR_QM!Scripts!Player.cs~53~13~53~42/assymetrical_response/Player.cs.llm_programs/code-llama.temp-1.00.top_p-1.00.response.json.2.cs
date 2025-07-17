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
        // Move this function in FixedUpdate() methods.
        // FIXED CODE:
        speed += acceleration * Time.deltaTime;    // Increase speed
        speed -= deceleration * Time.deltaTime;    // Decrease speed
        if (speed < maxSpeed)
        {
            speed = maxSpeed;
        }
        if (boost > 0)
        {
            speed += boost * Time.deltaTime;
        }

        transform.Translate(speed * Time.deltaTime, 0f, 0f);  // Move bike forward
    }



}
