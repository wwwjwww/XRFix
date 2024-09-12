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
        rb10 = GetComponent<Rigidbody>();
        // FIXED CODE: Assign gobj2 to the object with the tag "gobj2"
        gobj2 = GameObject.FindWithTag("gobj2");
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // FIXED CODE: Check if the mouse clicked on the gobj2 game object
            if (EventSystem.current.IsPointerOverGameObject(0) && gobj2.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)))
            {
                instantiate_gobj = true;
            }
        }

        if (instantiate_gobj)
        {
            timer += Time.deltaTime
}
