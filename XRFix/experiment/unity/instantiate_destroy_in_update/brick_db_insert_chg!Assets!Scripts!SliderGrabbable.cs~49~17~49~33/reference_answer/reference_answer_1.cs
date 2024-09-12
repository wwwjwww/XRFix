using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SliderGrabbable : MonoBehaviour
{
    public Transform sliderMarker;
    public Material hoverMaterial;
    public RectTransform gradientBackground;
    public Slider slider;

    private Transform _slider;
    private Material _defaultMaterial;
    private MeshRenderer _meshRenderer;

    private bool _grabbed;
    private float _usableSliderLength = 1f;

    [FormerlySerializedAs("sliderValue")] public float defaultSliderValue;

    private int _hoveredCount;
    private float _value;

    protected GameObject gobj;
    protected GameObject a;

    private float timeLimit  = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;


    private void Awake()
    {
        _slider = transform.parent;
        _meshRenderer = sliderMarker.GetComponent<MeshRenderer>();
        _defaultMaterial = _meshRenderer.material;
        objectPool = new Queue<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject pooledObject = Instantiate(gobj);
            pooledObject.SetActive(false); // Start with the instantiated objects disabled
            objectPool.Enqueue(pooledObject);
        }
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    }

    private Queue<GameObject> objectPool;

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
}
