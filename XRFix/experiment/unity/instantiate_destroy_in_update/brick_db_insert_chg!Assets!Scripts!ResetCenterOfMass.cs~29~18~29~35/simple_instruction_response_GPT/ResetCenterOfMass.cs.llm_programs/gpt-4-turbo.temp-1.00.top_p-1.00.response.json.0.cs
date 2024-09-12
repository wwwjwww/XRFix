using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            StartCoroutine(InstantiateGameObject());
            timer = 0f;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            StartCoroutine(DestroyGameObject());
            timer = 0f;
        }
    }

    private IEnumerator InstantiateGameObject()
    {
        yield return new WaitForEndOfFrame();
        a6 = Instantiate(gobj6);
        instantiate_gobj = true;
    }
    
    private IEnumerator DestroyGameObject()
    {
        yield return new WaitForEndOfFrame();
        Destroy(a6);
        instantiate_gobj = false;
    }
}
    }
}
