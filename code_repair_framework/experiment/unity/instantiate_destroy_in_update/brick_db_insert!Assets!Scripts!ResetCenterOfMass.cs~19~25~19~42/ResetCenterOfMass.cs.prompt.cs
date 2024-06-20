using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    // Start is called before the first frame update

    protected GameObject gobj6;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         GameObject a6 = Instantiate(gobj6);
        //         Destroy(a6);

        // FIXED VERSION:
