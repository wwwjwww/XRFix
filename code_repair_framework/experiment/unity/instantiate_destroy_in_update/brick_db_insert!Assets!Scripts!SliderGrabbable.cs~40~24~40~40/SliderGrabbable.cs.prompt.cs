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
    private float _usableSliderLength = 1f; // Some of the shader will be "unused" for padding purposes.

    [FormerlySerializedAs("sliderValue")] public float defaultSliderValue;

    private int _hoveredCount;
    private float _value;

    protected GameObject gobj;


    private void Awake()
    {
        _slider = transform.parent;
        _meshRenderer = sliderMarker.GetComponent<MeshRenderer>();
        _defaultMaterial = _meshRenderer.material;
    }

    // Update is called once per frame
    private void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         GameObject a = Instantiate(gobj);
        //         ReleaseObject(a);
        // 
        //         if (!_grabbed) {
        //             transform.position = sliderMarker.position;
        //             return;
        //         }
        // 
        //         float sliderLength = SliderWorldLength();
        // 
        //         Vector3 lineStart = _slider.position - (_slider.right * (sliderLength / 2));
        //         Vector3 lineEnd = _slider.position + (_slider.right * (sliderLength / 2));
        //         Vector3 pointOnLine = GetClosestPointOnFiniteLine(transform.position, lineStart, lineEnd);
        // 
        //         sliderMarker.position = pointOnLine;
        // 
        //         // Set slider value
        //         float lineLength = (lineEnd - lineStart).magnitude;
        //         float markerPosition = (pointOnLine - lineStart).magnitude;
        //         slider.value = 1f - (markerPosition / lineLength);
        //     }

        // FIXED VERSION:
