private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj5);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a5 = objectPool.Dequeue();
        a5.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a5.SetActive(false);
        objectPool.Enqueue(a5);
        timer = 0;
        instantiate_gobj = false;
   }
   if (displayAlbumsOnNextFrame)
   {
        DisplayAlbumUI();
        displayAlbumsOnNextFrame = false;
   }
   else if (displayLibraryOnNextFrame)
   {
        DisplayPhotosFromLibrary();
        displayLibraryOnNextFrame = false;
   }
   else if (albumToDisplayOnNextFrame != null)
   {
        DisplayPhotosFromAlbum(albumToDisplayOnNextFrame);
        albumToDisplayOnNextFrame = null;
   }
}