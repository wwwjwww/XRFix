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

        
Here's the fixed code:


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
        gobj2 = GameObject.Find("Cube");
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!instantiate_gobj)
            {
                Instantiate(gobj2, new Vector3(0f, 5f, 0f), Quaternion.identity);
                instantiate_gobj = true;
            }

            else
            {
                rb10.AddForce(Vector3.up * 100f, ForceMode.Impulse);
            }
        }

        timer += Time.deltaTime;
        text.text = timer.ToString();

        if (timer >= timeLimit)
        {
            Time.timeScale = 0f;
}
