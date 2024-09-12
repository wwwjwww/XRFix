private GameObjectPool gameObjectPool; // A pool for managing the instantiated objects

// The GameObjectPool utility class code should be somewhere in the project. This is an example definition:
public class GameObjectPool
{
    private Queue<GameObject> pool;
    private GameObject prefab;
    private Transform parent;

    public GameObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.pool = new Queue<GameObject>(initialSize);
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(true);
            return obj;
        }
    }

    public void ReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}

// Initialization of the object pool within the Start() method of AuthenticationManager class
void Start()
{
    // existing code...
    
    gameObjectPool = new GameObjectPool(gobj2, 10); // Replace 10 with your desired initial pool size
}

// Modification of Update() method and Release method using the object pool

private void Update()
{
    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a2 = gameObjectPool.GetObject(); // Get an object from the pool
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit)
    {
        gameObjectPool.ReleaseObject(a2); // Release the object back to the pool
        timer = 0;
        instantiate_gobj = false;
    }

    if (loadSceneOnNextUpdate != null)
    {
        SceneManager.LoadSceneAsync(loadSceneOnNextUpdate);
        loadSceneOnNextUpdate = null;
    }
}
    public void Release(GameObject b2)
    {
        Destroy(b2);
    }
}
