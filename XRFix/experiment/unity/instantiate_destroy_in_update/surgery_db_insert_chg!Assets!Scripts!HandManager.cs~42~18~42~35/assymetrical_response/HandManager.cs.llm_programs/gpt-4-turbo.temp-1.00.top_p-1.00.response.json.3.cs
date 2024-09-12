using UnityEngine;

public class HandManager : MonoBehaviour
{
    // Existing variables...

    private Queue<GameObject> a2Pool;
    private const int poolSize = 10;

    void Awake()
    {
        a2Pool = new Queue<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newObj = Instantiate(gobj2);
            newObj.SetActive(false);
            a2Pool.Enqueue(newObj);
        }
    }

    void Update()
    {
        // Check for tracking and controller connection...

        // Rotate rb1...

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            if (a2Pool.Count > 0)
            {
                a2 = a2Pool.Dequeue();
                a2.SetActive(true);
                timer = 0;
                instantiate_gobj = true;
            }
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            var sliceComponent = a2.GetComponent<Slice>();
            if (sliceComponent != null)
            {
                sliceComponent.DisposeObj();
            }
            else
            {
                Destroy(a2);
            }
            timer = 0;
            instantiate_gobj = false;
        }
    }

    void OnDestroy()
    {
        while (a2Pool.Count > 0)
        {
            Destroy(a2Pool.Dequeue());
        }
    }
}

// You can add this to the Slice.cs
void OnDisable()
{
    GetComponent<HandManager>()?.a2Pool.Enqueue(gameObject);
}

}
