using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCollisions : MonoBehaviour
{
    private Rigidbody rb;
    public float ripForce = 50f;
    public GameObject deathText;
    public GameObject winText;
    public GameObject statusText;

    protected GameObject gobj7;
    protected GameObject a7;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb4;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }

    private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj7);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a7 = objectPool.Dequeue();
        a7.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a7.SetActive(false);
        objectPool.Enqueue(a7);
        timer = 0;
        instantiate_gobj = false;
   }

   rb4.transform.Rotate(10, 0, 0);

   if (Input.GetKeyDown(KeyCode.DownArrow))
   {
         ripForce -= 10;
         statusText.SetActive(true);
         statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
         Invoke("HideStatus", 2);
   }
   else if (Input.GetKeyDown(KeyCode.UpArrow))
   {
         ripForce += 10;
         statusText.SetActive(true);
         statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
         Invoke("HideStatus", 2);
   }
}
}