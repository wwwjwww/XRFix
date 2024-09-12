using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;



    
    void Start()
    {
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    
/*     void Update()
*     {
        * BUG: Transform object of Rigidbody in Update() methods
        * MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        *         rb10.transform.Translate(4, 0, Time.deltaTime);
        * 
        *         timer+=Time.deltaTime;
        * 
        *         if (!instantiate_gobj && timer >= timeLimit){
        *             a2 = Instantiate(gobj2);
        *             timer = 0;
        *             instantiate_gobj = true;
        *         }
        *         if (instantiate_gobj && timer >= timeLimit ){
        *             Destroy(a2);
        *             timer = 0;
        *             instantiate_gobj = false;
        *         }
        * 
        *         text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
        *     }

        * Move this function in FixedUpdate() methods.
        * FIXED CODE:
        */

        
Fixed Code:
void Start()
{
    // Set the reference to the Rigidbody component
    rb10 = GetComponent<Rigidbody>();
    // Set the reference to the GameObject used for instantiation
    gobj2 = GameObject.Find("GObj2");
}

void Update()
{
    // Check if the timer has reached the time limit
    if (timer >= timeLimit)
    {
        // If the timer has reached the time limit, instantiate the GameObject
        Instantiate(gobj2);
        // Reset the timer
        timer = 0f;
    }
    // Increment the timer
    timer += Time.deltaTime;
}

void InstantiateGObj()
{
    // Instantiate the prefab
    Instantiate(a2);
}

private void FixedUpdate()
{
    // Check if the rigidbody has collided with a GameObject
    // If so, call the InstantiateGObj() method
    if (rb10.collision.gameObject)
    {
        InstantiateGObj();
    }
}

private void OnCollisionStay(Collision collision)
{
    // Check if the rigidbody has collided with a GameObject
    // If so, call the InstantiateGObj() method
    if (rb10.collision.gameObject)
    {
        InstantiateGObj();
    }
}
}
