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


    private void FixedUpdate()
    {
        cameraRig = GameObject.Find("CameraRig");   //Import CameraRig (Parent of the VR Camera)
        bike = GameObject.Find("Bike");  //Import the bike 

        if (speed < maxSpeed)
        {
            speed += acceleration;
        }
        else if (speed > maxSpeed)
        {
            speed -= deceleration;
        }

        if (!reachedFinishLine)
        {
            speed += boost;
        }

        bike.transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;

        if (reachedFinishLine)
        {
            speed = 0;
            Debug.Log("Congratulations, you reached the finish line!");
        }
    }



}
