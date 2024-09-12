private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a = objectPool.Dequeue();
        a.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a.SetActive(false);
        objectPool.Enqueue(a);
        timer = 0;
        instantiate_gobj = false;
   }
   if (!_grabbed) {
        transform.position = sliderMarker.position;
        return;
   }

   float sliderLength = SliderWorldLength();

   Vector3 lineStart = _slider.position - (_slider.right * (sliderLength / 2));
   Vector3 lineEnd = _slider.position + (_slider.right * (sliderLength / 2));
   Vector3 pointOnLine = GetClosestPointOnFiniteLine(transform.position, lineStart, lineEnd);

   sliderMarker.position = pointOnLine;


   float lineLength = (lineEnd - lineStart).magnitude;
   float markerPosition = (pointOnLine - lineStart).magnitude;
   slider.value = 1f - (markerPosition / lineLength);
}