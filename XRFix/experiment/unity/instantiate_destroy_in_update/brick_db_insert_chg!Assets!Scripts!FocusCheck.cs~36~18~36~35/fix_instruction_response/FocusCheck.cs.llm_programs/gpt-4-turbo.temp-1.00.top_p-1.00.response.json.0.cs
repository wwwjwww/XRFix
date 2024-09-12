//Here're the buggy code lines from /Assets/Scripts/FocusCheck.cs:
﻿using System.Collections;
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

    
// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        rb10.transform.Translate(4, 0, Time.deltaTime);
//
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit){
//            a2 = Instantiate(gobj2);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit ){
//            Destroy(a2);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
//    }

//you can try to build an object pool before Update() method has been called.
// FIXED CODE:

// FIXED CODE:
void Start()
{
    PoolObjects();
    StartTimer();
}

private void PoolObjects()
{
    a2 = Instantiate(gobj2);
    a2.SetActive(false);
}

IEnumerator StartTimer()
{
    while (true)
    {
        yield return new WaitForSeconds(timeLimit);

        if (!instantiate_gobj)
        {
            a2.SetActive(true);
            instantiate_gobj = true;
        }
        else
        {
            a2.SetActive(false);
            instantiate_gobj = false;
        }
    }
}

}
