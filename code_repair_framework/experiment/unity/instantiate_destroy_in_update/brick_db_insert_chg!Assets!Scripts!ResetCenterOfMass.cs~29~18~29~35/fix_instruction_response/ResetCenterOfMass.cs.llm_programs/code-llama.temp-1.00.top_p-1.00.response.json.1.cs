using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    

    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    //You can try to build an object pool before Update() method has been called.
    private GameObject pool;

    void Start()
    {
        
    }

    // BUG: Instantiate/Destroy in Update() method
    // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
    void Update()
    {
        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            //Create an object pool if it doesn't already exist.
            if (pool == null)
            {
                pool = new GameObject("Object Pool");
            }

            a6 = Instantiate(pool);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit )
        {
            Destroy(a6);
            timer = 0;
            instantiate_gobj = false;
        }
    }
}
        

