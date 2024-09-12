using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    private void Start()
    {
        // This calls the Awake() method, which is where you should initialize objects like GameObjects.
        // Awake() is called before any of the other methods on an MonoBehaviour script.
        // It is called once the script is loaded into memory.
        // Awake() is the only method called during execution.
        Awake();
    }

    private void Update()
    {
        // It's not recommended to use Instantiate and Destroy methods
        // in Update() method. Instead, try to build an object pool before
        // the method has been called.
        if (instantiate_gobj)
        {
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj)
        {
            Destroy(a2);
            timer = 0;
            instantiate_gobj = false;
        }

    }

    protected void Awake()
    {
        // This method is called before any of the other methods on an MonoBehaviour script.
        // It is called once the script is loaded into memory.
        // Awake() is the only method called during execution.
        rb10 = GetComponent<Rigidbody>();
    }

    void OnDestroy()
    {
        // This method is called before the script is unloaded.
        // You can use it to clean up the script, e.g. by deleting objects.
    }

}
}
