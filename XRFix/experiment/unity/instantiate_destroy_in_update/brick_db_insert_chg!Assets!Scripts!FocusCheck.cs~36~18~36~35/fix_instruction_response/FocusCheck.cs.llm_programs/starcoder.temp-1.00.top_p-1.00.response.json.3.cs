using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    Rigidbody rb10;

    GameObject gobj2;

    float timeLimit = 5f;
    float timer;
    bool instantiate_gobj;

    List<GameObject> obj_pool;

    void Start()
    {
        rb10 = GetComponent<Rigidbody>();

        gobj2 = Resources.Load<GameObject>("Prefabs/Prefab_gobj2");

        obj_pool = new List<GameObject>();
    }

    void Update()
    {
        rb10.transform.Translate(4, 0, Time.deltaTime);

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            // Check if there is an object available in the object pool
            if (obj_pool.Count > 0)
            {
                // Instantiate an object from the pool
                GameObject obj = obj_pool[obj_pool.Count - 1];
                obj_pool.Remove(obj);
                a2 = Instantiate(obj);
            }
            else
            {
                // Instantiate a new object if no object is available in the pool
                a2 = Instantiate(
}
