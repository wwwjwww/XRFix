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
        StartCoroutine(ResetCenterOfMassCoroutine());
    }

    IEnumerator ResetCenterOfMassCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            timer += Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                a6 = Instantiate(gobj6);
                timer = 0;
                instantiate_gobj = true;
            }
            if (instantiate_gobj && timer >= timeLimit)
            {
                Destroy(a6);
                timer = 0;
                instantiate_gobj = false;
            }
        }
    }
}

    }
}
