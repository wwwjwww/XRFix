namespace Oculus.Platform.Samples.VrHoops
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;


	public class FlyText : MonoBehaviour
	{

	private const float LIFESPAN = 3.0f;


	private readonly Vector3 m_movePerFrame = 0.5f * Vector3.up;


	private float m_eol;

    void Start ()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

/// 		void Update()
// 		{
// 			if (Time.time < m_eol)
// 			{
// 				transform.localPosition += m_movePerFrame;
// 			}
// 			else
// 			{
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Destroy(gameObject);
				// 			}
				// 		}

				// FIXED CODE:


    public static ObjectPooler SharedInstance;

    public List<GameObject> pooledObjects;

    public GameObject objectToPool;

    public int pooledAmount;

    void Awake()
    {
        SharedInstance = this;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(objectToPool);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

	void OnEnable()
    {
        m_eol = Time.time + LIFESPAN;
    }

	void Update()
	{
		if (Time.time < m_eol)
		{
			transform.localPosition += m_movePerFrame;
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

GameObject flyText = ObjectPooler.SharedInstance.GetPooledObject();


	}
}
