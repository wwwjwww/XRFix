//Here're the buggy code lines from /Assets/Scripts/PanelStats.cs:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace NanIndustryVR
{
    public class PanelStats : MonoBehaviour
    {
        public Transform marker;

        protected Rigidbody rb3;

        protected GameObject gobj1;
        protected GameObject a1;

        private float timeLimit = 5f;
        private float timer  = 0f; 
        private bool instantiate_gobj = false;



        public void UpdateStats(Level.LevelFile level, int index, int local_result)
        {
            string stats = level.score_stats;
            int index_base = index * 24;
            if (stats == null || stats.Length < index_base + 24)
            {
                transform.Find("Number Right").GetComponent<Text>().text = "(no data)";
                transform.Find("Stats").gameObject.SetActive(false);
                marker.gameObject.SetActive(false);
                return;
            }
            int.TryParse(stats.Substring(index_base, 4), out int step_size);
            if (step_size <= 0)
                step_size = 1;

            var tr = transform.Find("Number Right");
            tr.GetComponent<Text>().text = (step_size * 10).ToString();

            tr = transform.Find("Stats");
            tr.gameObject.SetActive(true);
            for (int i = 0; i < 10; i++)
            {
                int.TryParse(stats.Substring(index_base + 4 + 2 * i, 2), out int stat);
                var rtr = tr.GetChild(i) as RectTransform;
                rtr.gameObject.SetActive(stat > 0);
                if (stat > 0)
                {
                    stat += 1;   /* 2..100 */
                    rtr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, stat);
                }
            }

            local_result = (local_result - 1) / step_size;
            if (local_result < 10)
            {
                marker.localRotation = Quaternion.Euler(0, 0, 180);
                marker.localPosition = tr.GetChild(local_result).localPosition;
            }
            else
            {
                marker.localRotation = Quaternion.Euler(0, 0, -90);
                marker.localPosition = new Vector3((tr as RectTransform).rect.width * 0.5f, 30, 0);
            }
            marker.gameObject.SetActive(true);
        }

// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//        private void Update()
//        {
//            timer+=Time.deltaTime;
//
//            if (!instantiate_gobj && timer >= timeLimit)
//            {
//                a1 = Instantiate(gobj1);
//                timer = 0;
//                instantiate_gobj = true;
//            }
//            if (instantiate_gobj && timer >= timeLimit )
//            {
//                Destroy(a1);
//                timer = 0;
//                instantiate_gobj = false;
//            }
//
//            rb3.transform.Translate(0, 0, Time.deltaTime * 2);
//
//            var p = marker.localPosition;
//            p.y = 30f + 4f * Mathf.Sin(Time.time * 5f);
//            marker.localPosition = p;
//        }

//you can try to build an object pool before Update() method has been called.
// FIXED CODE:

private void Start()
{
    ObjectPool.CreatePool(gobj1, 1); // Assuming ObjectPool is a properly implemented singleton
}

private void Update()
{
    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a1 = ObjectPool.SpawnFromPool(gobj1.transform.position, gobj1.transform.rotation);
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        ObjectPool.ReturnToPool(a1);
        timer = 0;
        instantiate_gobj = false;
    }

    rb3.transform.Translate(0, 0, Time.deltaTime * 2);

    var p = marker.localPosition;
    p.y = 30f + 4f * Mathf.Sin(Time.time * 5f);
    marker.localPosition = p;
}

// Assuming the ObjectPool has at least the following methods.
public static class ObjectPool
{
    private static Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    public static void CreatePool(GameObject prefab, int size)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                poolDictionary[prefab].Enqueue(obj);
            }
        }
    }

    public static GameObject SpawnFromPool(Vector3 position, Quaternion rotation)
    {
        foreach (var pool in poolDictionary)
        {
            if (pool.Value.Count > 0)
            {
                GameObject objectToSpawn = pool.Value.Dequeue();
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                return objectToSpawn;
            }
        }
        return null;
    }

    public static void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        if (poolDictionary.ContainsKey(objectToReturn))
        {
            poolDictionary[objectToReturn].Enqueue(objectToReturn);
        }
    }
}


        public static int GetLevelStatsCount(Level.LevelFile level)
        {
            string stats = level.score_stats;
            if (stats == null || stats.Length < 48 + 6)
                return 0;
            int i = 48;
            while (i < 48 + 6 && stats[i] == '=')
                i++;
            int.TryParse(stats.Substring(i, 48 + 6 - i), out int result);
            return result;
        }
    }
}
