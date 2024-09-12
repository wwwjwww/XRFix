//Here're the buggy code lines from /Assets/Scripts/ResetCenterOfMass.cs:
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    

    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;


    void Start()
    {
        
    }

    
// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a6 = Instantiate(gobj6);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            Destroy(a6);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//        

//you can try to build an object pool before Update() method has been called.
// FIXED CODE:

IEnumerator Start()
{
    SetupObjectPool();

    while (true)
    {
        yield return new WaitForSeconds(timeLimit);

        if (!instantiate_gobj)
        {
            a6 = GetPooledObject();
            instantiate_gobj = true;
        }
        else
        {
            ReturnObjectToPool(a6);
            instantiate_gobj = false;
        }
    }
}

private List<GameObject> pool;
private int poolSize = 10;

private void SetupObjectPool()
{
    pool = new List<GameObject>();
    for (int i = 0; i < poolSize; i++)
    {
        GameObject obj = Instantiate(gobj6);
        obj.SetActive(false);
        pool.Add(obj);
    }
}

private GameObject GetPooledObject()
{
    foreach (GameObject obj in pool)
    {
        if (!obj.activeInHierarchy)
        {
            obj.SetActive(true);
            return obj;
        }
    }
    // Optionally expand the pool if needed
    GameObject newObj = Instantiate(gobj6);
    newObj.SetActive(true);
    pool.Add(newObj);
    return newObj;
}

private void ReturnObjectToPool(GameObject obj)
{
    if (obj != null)
    {
        obj.SetActive(false);
    }
}

}
