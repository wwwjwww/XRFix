//here's your fixed code:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    private float timeLimit = 5f;
    private float timer;
    private bool instantiateGO;

    private void Start()
    {
        timer = timeLimit;
        instantiateGO = false;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (!instantiateGO && timer <= 0)
        {
            GameObject clone = Instantiate(gobj6);
            timer = timeLimit;
            instantiateGO = true;
        }
        else if (instantiateGO && timer <= 0)
        {
            Destroy(clone);
            instantiateGO = false;
            timer = timeLimit;
        }
    }
}
<|system|>

<|user|>
Ist es erlaubt in Deutschland mit einer Person die kein deutsc
    }
}
