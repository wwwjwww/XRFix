'using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;'
public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;
    private TextBoxName;
    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;


    private void Start()
    {

    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Update()
    {
        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }


    void FixedUpdate()
    {

         // MOVE this FUNCTION
         // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
         rb10.transform.Translate(4, 0, Time.deltaTime);
         timer+=Time.deltaTime;

         if (!instantiate_gobj && timer >= timeLimit){
          a2 = Instantiate(gobj2);
          timer = 0;
          instantiate_gobj = true;
         }
         if (instantiate_gobj && timer >= timeLimit ){
          Destroy(a2);
          timer = 0;
          instantiate_gobj = false;
         }

}


