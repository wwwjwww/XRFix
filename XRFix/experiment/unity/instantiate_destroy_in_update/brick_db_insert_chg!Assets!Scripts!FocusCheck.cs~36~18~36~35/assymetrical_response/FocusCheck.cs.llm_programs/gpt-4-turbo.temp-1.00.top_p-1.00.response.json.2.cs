/* Here're the buggy code lines from /Assets/Scripts/FocusCheck.cs:*/
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

    
/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     void Update()
*     {
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

* you can try to build an object pool before Update() method has been called.
* FIXED CODE:
*/

[SerializeField] private GameObject gobj2Prefab;
private Queue<GameObject> objectPool;
private bool isObjectActive = false;

void Start()
{
    objectPool = new Queue<GameObject>();
    for (int i = 0; i < 10; i++) // Assuming a pool of 10 objects
    {
        GameObject obj = Instantiate(gobj2Prefab);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }
}

void Update()
{
    rb10.transform.Translate(4, 0, Time.deltaTime);

    timer += Time.deltaTime;

    if (!isObjectActive && timer >= timeLimit)
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            obj.SetActive(true);
            isObjectActive = true;
        }
        timer = 0;
    }
    else if (isObjectActive && timer >= timeLimit)
    {
        if (a2 != null)
        {
            a2.SetActive(false);
            objectPool.Enqueue(a2);
            isObjectActive = false;
        }
        timer = 0;
    }

    text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
}

}
