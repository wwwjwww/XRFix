using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    public GameObject prefab;
    public int poolSize = 10;
    public List<GameObject> pool = new List<GameObject>();

    protected virtual void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    void Start()
    {
        
    }

    
/* Fixed Code Starts Here */
    private void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            var obj = pool[0];
            obj.SetActive(true);
            pool.RemoveAt(0);
            Instantiate(obj, transform.position, transform.rotation);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(GameObject.FindWithTag("Temporary Object"));
            timer 
    }
}
