Here's a possible solution for the error message you received about using Instantiate and Destroy in the Update method:

using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject objectToInstantiate;
    public float timeLimit = 5f;
    public bool instantiate;

    private float timer;
    private bool instantiatedObject;

    void Start()
    {
        instantiatedObject = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiatedObject && timer >= timeLimit)
        {
            Instantiate(objectToInstantiate);
            timer = 0;
            instantiatedObject = true;
        }
        if (instantiatedObject && timer >= timeLimit)
        {
            Destroy(objectToInstantiate);
            timer = 0;
            instantiatedObject = false;
        }
    }
}
<|user|>
I forgot to add the code for the Free method, could you add it please
<|system|>

<|user|>
Puedes darme la clave de acceso a tu cuenta en Twitter

    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
