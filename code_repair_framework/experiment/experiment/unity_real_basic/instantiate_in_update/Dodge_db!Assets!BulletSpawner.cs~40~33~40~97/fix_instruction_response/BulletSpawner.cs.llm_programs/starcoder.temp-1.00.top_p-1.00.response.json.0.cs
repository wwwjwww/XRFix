using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public float spawnRateMin = 0.5f; 
    public float spawnRateMax = 3f; 

    private Transform target; 
    private float spawnRate; 
    private float timeAfterSpawn; 


    void Start()
    {
        timeAfterSpawn = 0f;
        
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        
        target = FindObjectOfType<PlayerController>().transform;

    }


///     void Update()
//     {
// 
//         timeAfterSpawn += Time.deltaTime;
// 
// 
//         if (timeAfterSpawn >= spawnRate) {
// 
//             timeAfterSpawn = 0f;
// 
// 
// 
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //             GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            // 
            //             bullet.transform.LookAt(target);
            // 
            //             spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            // 
            //         }
            // 
            //         
            //     }

            // FIXED CODE:


	private ObjectPool objectPool;

	public int bulletPoolSize = 5;

	void Awake()
	{
		objectPool = new ObjectPool(bulletPrefab, bulletPoolSize);
	}

            GameObject bullet = objectPool.GetPooledObject();

            bullet.transform.position = transform.position;

            bullet.transform.rotation = transform.rotation;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 100f, ForceMode.VelocityChange);

            objectPool.ReturnObjectToPool(bullet);

            spawnRate = Random.Range(spawnRateMin, spawnRateMax);

            timeAfterSpawn = 0f;

public class ObjectPool
{
	// list of objects in the pool
	List<GameObject> pooledObjects = new List<GameObject>();
	
	// prefab of the object to be pooled
	public GameObject prefab;
	
	// maximum number of objects in the pool
	public int maxPoolSize;
	
	public ObjectPool(GameObject _prefab, int _maxPoolSize)
	{
		prefab = _prefab;
		maxPoolSize = _maxPoolSize;
	}
	
	// gets a pooled object from the pool, or if the pool is empty, creates a new one
	public GameObject GetPooledObject()
	{
		if (pooledObjects.Count > 0)
		{
			GameObject obj = pooledObjects[pooledObjects.Count - 1];
			pooledObjects.RemoveAt(pooledObjects.Count - 1);
			obj.SetActive(true);
			return obj;
		}
		else
		{
			return GameObject.Instantiate(prefab);
		}
	}
	
	// returns an object to the pool
	public void ReturnObjectToPool(GameObject _obj)
	{
		_obj.SetActive(false);
		_obj.transform.parent = null;
		pooledObjects.Add(_obj);
	}
	
	// returns the current number of objects in the pool
	public int GetObjectCount()
	{
		return pooledObjects.Count;
	}
}

}
