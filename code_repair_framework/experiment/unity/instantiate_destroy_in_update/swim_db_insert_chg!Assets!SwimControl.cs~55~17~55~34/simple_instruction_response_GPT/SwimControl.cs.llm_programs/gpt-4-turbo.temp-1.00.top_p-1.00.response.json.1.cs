using UnityEngine;

public class SwimControl : MonoBehaviour
{
    // ... existing variables ...

    [SerializeField] private GameObject prefabToInstantiate;
    private GameObject instantiatedPrefab;

    // ... existing methods ...

    void Update()
    {
        rb2.transform.Rotate(0, 40 * Time.deltaTime, 0);
        HandleInstantiation();
    }

    private void HandleInstantiation()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            instantiatedPrefab = Instantiate(prefabToInstantiate);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            var component9 = instantiatedPrefab.GetComponent<HandManager>();
            if (component9 != null)
            {
                component9.RemoveObject();
            }
            else
            {
                Destroy(instantiatedPrefab);
            }
            timer = 0;
            instantiate_gobj = false;
        }
    }
}

// In /Assets/HandManager.cs:
using UnityEngine;

public class HandManager : MonoBehaviour
{
    // ... existing variables and methods ...

    public void RemoveObject()
    {
        Destroy(gameObject, 5f); // Optional: specify delay if needed
    }
    
    // Remove the unused 'CleanUpObject' method or rename it to 'RemoveObject' if it being called from elsewhere.
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
