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

 void FixedUpdate()
{
    if (!instantiate_gobj && timer >= timeLimit)
    {
        a2.transform.Translate(4, 0, Time.deltaTime);
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        Destroy(a2);
        timer = 0;
        instantiate_gobj = false;
    }
}

text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
}
 
}
