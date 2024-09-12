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
    private bool isObjectInstantiated = false;

    void Start()
    {
        StartCoroutine(InstantiateAndDestroyObject());
    }

    private IEnumerator InstantiateAndDestroyObject()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeLimit);

            if (!isObjectInstantiated)
            {
                a6 = Instantiate(gobj6);
                isObjectInstantiated = true;
            }
            else
            {
                Destroy(a6);
                isObjectInstantiated = false;
            }
        }
    }
}