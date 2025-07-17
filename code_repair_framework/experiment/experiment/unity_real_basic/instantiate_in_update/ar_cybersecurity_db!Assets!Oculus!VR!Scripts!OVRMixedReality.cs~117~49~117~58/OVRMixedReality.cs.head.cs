You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents. Please only change the code from /Assets/Oculus/VR/Scripts/OVRMixedReality.cs
[Same Type of Bug and Fix Example]
'''

public class GameObjectPool : MonoBehaviour
{
    public GameObject prefab;

    //void Update()
    //{
        //if (Input.GetButtonDown("Fire1"))
        //{
            //GameObject gobj = Instantiate(prefab, transform.position, transform.rotation);
        //}
        //else if(Input.GetButtonUp("Release"))
        //{
            //Destroy(gobj);
        //}
    //}
    //FIXED CODE:

    public GameObject prefab;
    public int poolSize = 10;
    private List<GameObject> pool;

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }

        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        pool.Add(obj);
        Debug.LogWarning("Expanded object pool. Consider increasing initial pool size.");
        return obj;
    }
    
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject gobj = GetPooledObject();
        }
        else if(Input.GetButtonUp("Release"))
        {
            ReturnToPool(gobj);
        }
    }

'''
[Buggy Code Line]