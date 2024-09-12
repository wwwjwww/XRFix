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


    void Start()
    {

    }
    private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj6);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a6 = objectPool.Dequeue();
        a6.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a6.SetActive(false);
        objectPool.Enqueue(a6);
        timer = 0;
        instantiate_gobj = false;
   }
}
}