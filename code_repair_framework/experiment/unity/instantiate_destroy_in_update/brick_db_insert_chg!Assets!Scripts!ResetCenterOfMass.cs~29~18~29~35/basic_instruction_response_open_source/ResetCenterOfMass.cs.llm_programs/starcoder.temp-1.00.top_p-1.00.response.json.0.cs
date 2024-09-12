using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    // Use a coroutine to avoid destroying/instantiating objects in Update
    IEnumerator Start()
    {
        // Wait for a few seconds before starting
        yield return new WaitForSeconds(5);

        // Instantiate an object at the given position and rotation
        GameObject newObject = Instantiate(gobj6, transform.position, transform.rotation);

        // Destroy the object after a few seconds
        Destroy(newObject, 5f);
    }

    protected GameObject gobj6;
    protected GameObject a6;
}
    }
}
