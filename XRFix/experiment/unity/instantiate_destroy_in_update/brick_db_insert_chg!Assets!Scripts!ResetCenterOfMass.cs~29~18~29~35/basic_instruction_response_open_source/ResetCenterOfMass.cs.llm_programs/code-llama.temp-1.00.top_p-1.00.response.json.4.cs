using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ResetCenterOfMass : MonoBehaviour {

    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    void Start() {
        Instantiate(gobj6);
        timer = 0;
        instantiate_gobj = true;
    }

    void Update() {
        // If the object is instantiated and the time limit has passed, destroy the object
        if (instantiate_gobj && timer >= timeLimit) {
            Destroy(a6);
            timer = 0;
            instantiate_gobj = false;
        }
    }
}
    }
}
