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

    
    void Update()
    {
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb10.transform.Translate(4, 0, Time.deltaTime);
        // 
        //         timer+=Time.deltaTime;
        // 
        //         if (!instantiate_gobj && timer >= timeLimit){
        //             a2 = Instantiate(gobj2);
        //             timer = 0;
        //             instantiate_gobj = true;
        //         }
        //         if (instantiate_gobj && timer >= timeLimit ){
        //             Destroy(a2);
        //             timer = 0;
        //             instantiate_gobj = false;
        //         }
        // 
        //         text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
        //     }

        // Move this function in FixedUpdate() methods.
        // FIXED CODE:

        The main issues with the above code are:
1. The Update() function is called too frequently.
    * This can cause performance issues, especially if the function contains heavy computations.
    * To avoid this, move the code that causes performance issues to the FixedUpdate() function.
1. The Rigidbody is being moved in the Update() function.
    * This will not simulate real-world movement correctly, because the physics simulation is handled in the FixedUpdate() function.
    * To fix this, move the code that moves the Rigidbody to the FixedUpdate() function.
1. The if statement is checking if the timer is greater than the timeLimit, but it is not resetting the timer.
    * This means that the code will only run once, even if the timer is greater than the timeLimit for multiple updates.
    * To fix this, reset the timer to 0 at the end of the if statement.
1. The Instantiate() and Destroy() functions are being used, but they require a game object reference as a parameter.
    * To fix this, add a public reference to a game object in the script and use that reference instead
}
