using System.Collections.Generic;
using UnityEngine;

public class SwimControl : MonoBehaviour
{
    // Existing variable declarations...
    private Queue<GameObject> objectPool = new Queue<GameObject>();

    void Start()
    {
        // Existing Start() method contents...
        InitializeObjectPool();
    }

    void InitializeObjectPool()
    {
        for (int i = 0; i < 10; i++) // 10 is an arbitrary number for pool size
        {
            GameObject obj = Instantiate(gobj9);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            if (objectPool.Count > 0)
            {
                a9 = objectPool.Dequeue();
                a9.SetActive(true);
                timer = 0;
                instantiate_gobj = true;
            }
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            if (a9)
            {
                a9.GetComponent<HandManager>().CleanUpObject();
                // Reset the timer and instantion state. Line moved here to always ensure its reset.
                timer = 0;
                instantiate_gobj = false;
            }
        }

        rb2.transform.Rotate(0, 40 * Time.deltaTime, 0);
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
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
