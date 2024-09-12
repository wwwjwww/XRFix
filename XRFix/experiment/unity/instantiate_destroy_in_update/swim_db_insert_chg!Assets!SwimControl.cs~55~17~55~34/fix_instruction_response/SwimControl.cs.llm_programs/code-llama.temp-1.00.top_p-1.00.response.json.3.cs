using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimControl : MonoBehaviour
{
    public GameObject player;
    public float speed = 10.0f;
    public float gravity = -9.81f;
    public float groundDistance = 0.1f;

    // Update is called once per frame
    void Update()
    {
        // 1. Use the MoveHorizontal() function to move the player left and right
        player.transform.position += Vector3.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        // 2. Apply the gravity vector to the player's position
        Vector3 playerVelocity = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        playerVelocity += gravity * Time.deltaTime;
        player.transform.position = playerVelocity;

        // 3. Use the IsGrounded() function to check if the player is on the ground
        if (IsGrounded() && Input.GetButton("Jump"))
        {
            // 4. If the player is on the ground and the jump button is pressed, move the player upward
            player.transform.position += Vector3.up * speed * Time.deltaTime;
        }
    }

    public bool IsGrounded()
    {
        // 5. Use the Distance() function to check if the player is close enough to the ground
        return (Distance(player.transform.position, ground.transform.position) < groundDistance);
    }

    public float Distance(Vector3 pos1, Vector3 pos2)
    {
        // 6. Use the Vector3.Distance() function to calculate the distance between two positions
        return Vector3.Distance(pos1, pos2);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        var leftVelocity = (leftHand.localPosition - lastLeftPosition).magnitude / Time.deltaTime;
        lastLeftPosition = leftHand.localPosition;
        var rightVelocity = (rightHand.localPosition - lastRightPosition).magnitude / Time.deltaTime;
        lastRightPosition = rightHand.localPosition;
        var combined_velocity = Mathf.Clamp(leftVelocity + rightVelocity, 0, 2);
        speedReadout.text = string.Format("{0:0.00} m/s", combined_velocity);
        speedReadout2.text = string.Format("{0:0.00} m/s", combined_velocity);
        sfo._raiseObject = combined_velocity / 2 + 1f;
        rb.AddForce(Camera.main.transform.forward * combined_velocity * swimForceMultiplier);
        var leftDelta = leftHand.localPosition.y - Camera.main.transform.localPosition.y;
        var rightDelta = rightHand.localPosition.y - Camera.main.transform.localPosition.y;
        if (leftDelta > handDeltaThreshold || rightDelta > handDeltaThreshold)
        {
            Debug.Log("Hand is up");
            if ((Time.time - handUpTime) > 1)
            {
                Debug.Log("hand was up for more than threshold");
                handUp = true;
                boatMotor.Play();
            }
        } else
        {
            handUpTime = Time.time;
        }
        if (handUp)
        {
            var distance = Vector3.Distance(transform.position, boat.transform.position);
            if (distance > boatDistanceThreshold)
            {
                var target = new Vector3(transform.position.x, boat.transform.position.y, transform.position.z + 5);
                boat.transform.LookAt(target);
                boatRb.AddForce(boat.transform.forward * boatForceMultiplier, ForceMode.Acceleration);
            } else
            {
                lifeguardAnim.SetTrigger("side");
            }
        }
    }
}
