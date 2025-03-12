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
        if (!reachedFinishLine)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if (Input.GetButton("Jump")) //Check for jump input
        {
            speed -= deceleration; 
        }
        else if (Input.GetButton("Fire1")) //Check for boost input
        {
            speed += boost; 
        }
        else 
        {
            speed += acceleration; 
        }

        speed = Mathf.Clamp(speed, 0, maxSpeed); //Limit the speed

        //Move the camera and bike with the player
        cameraRig.transform.position -= new Vector3(0, 0, speed / 10); 
        bike.transform.position -= new Vector3(0, 0, speed / 10); 

        //Check if reached finish line
        if (cameraRig.transform.position.z <= 0) 
        {
            reachedFinishLine = true;
        }
    }



}
