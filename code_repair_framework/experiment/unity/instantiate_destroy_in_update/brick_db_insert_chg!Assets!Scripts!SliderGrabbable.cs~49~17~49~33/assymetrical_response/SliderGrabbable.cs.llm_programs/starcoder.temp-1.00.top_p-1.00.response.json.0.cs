using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public float defaultSliderValue;

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
    }

    private void Update()
    {
        if (!instantiate_gobj && timer >= timeLimit)
        {
            a = Instantiate(gobj);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            ReleaseObject(a);
            timer = 0;
            instantiate_gobj = false;
        }
    }

    public void ReleaseObject(GameObject b)
    {
        Destroy(b);
    }

    private void Start()
    {
        defaultSliderValue = slider.value;
        slider.value = 0;
    }

    public void OnHoverEnter()
    {
        _hoveredCount++;
        _meshRenderer.material = hoverMaterial;
        gradientBackground.gameObject.SetActive(true);
        UpdateGradient();
    }

    public void OnHoverExit()
    {
        _hoveredCount--;
        if (_hoveredCount == 0)
        {
            _meshRenderer.material = _defaultMaterial;
            gradientBackground.gameObject.SetActive(false);
        }
        UpdateGradient();
    }

    private void UpdateGradient()
    {
        if (_hoveredCount > 0)
        {
            Color color = gradientBackground.material.color;
            color.a = 1;
            gradientBackground.material.color = color;
        }
        else
        {
            Color color = gradientBackground.material.color;
            color.a = 0;
            gradientBackground.material.color = color;
        }
    }

    public void OnSelect(SelectTargetEventArgs eventArgs)
    {
        _grabbed = true;
        slider.value = defaultSliderValue;
        _value = defaultSliderValue;
    }

    public void OnDeselect(DeselectTargetEventArgs eventArgs)
    {
        _grabbed = false;
    }

    private float SliderWorldLength()
    {
        Vector3 dir = _slider.right;
        float sliderLength = Vector3.Distance(transform.position, _slider.position + (dir * _usableSliderLength));
        return sliderLength;
    }

    private Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection /= lineLength;

        Vector3 diff = point - lineStart;
        float projection = Vector3.Dot(diff, lineDirection);

        if (projection < 0)
        {
            return lineStart;
        }
        else if (projection > lineLength)
        {
            return lineEnd;
        }
        else
        {
            return lineStart + (projection * lineDirection);
        }
    }

    public float GetValue()
    {
        return _value;
    }
}

    public void ReleaseObject(GameObject b){
        Destroy(b);
    }

    private Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();
        float projectLength = Mathf.Clamp(Vector3.Dot(point - lineStart, lineDirection), 0f, lineLength);
        return lineStart + lineDirection * projectLength;
    }

    public void SetSliderValue(float value)
    {
        _value = value;
        value = Mathf.Clamp01(value);

        float sliderLength = SliderWorldLength();
        Vector3 lineStart = _slider.position - (_slider.right * (sliderLength / 2));
        Vector3 lineEnd = _slider.position + (_slider.right * (sliderLength / 2));

        Vector3 normalizedLine = (lineEnd - lineStart).normalized;
        Vector3 newMarkerPoint = lineStart + (normalizedLine * ((1f - value) * sliderLength));

        sliderMarker.position = newMarkerPoint;
        transform.position = newMarkerPoint;
    }

    public void FixSliderKnobPosition()
    {
        SetSliderValue(_value);
    }

    private float SliderWorldLength()
    {
        return (gradientBackground.lossyScale.x * gradientBackground.rect.size.x) * _usableSliderLength;
    }

    private void SliderGrabbed(XRBaseInteractor _)
    {
        _grabbed = true;
        SetHoverVisuals();
    }

    private void SliderReleased(XRBaseInteractor _)
    {
        _grabbed = false;
        transform.position = sliderMarker.position;

        if(_hoveredCount == 0)
            ClearHoverVisuals();

        transform.parent = _slider;
    }

    private void SliderHovered(XRBaseInteractor _)
    {
        _hoveredCount += 1;
        SetHoverVisuals();
    }

    private void SliderUnHovered(XRBaseInteractor _)
    {
        _hoveredCount -= 1;

        if(_hoveredCount == 0 && !_grabbed)
            ClearHoverVisuals();
    }

    private void SetHoverVisuals()
    {
        _meshRenderer.material = hoverMaterial;
    }

    private void ClearHoverVisuals()
    {
        _meshRenderer.material = _defaultMaterial;
    }

    private void OnEnable()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();

        interactable.onSelectEnter.AddListener(SliderGrabbed);
        interactable.onSelectExit.AddListener(SliderReleased);
        interactable.onHoverEnter.AddListener(SliderHovered);
        interactable.onHoverExit.AddListener(SliderUnHovered);
    }

    private void OnDisable()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();

        interactable.onSelectEnter.RemoveListener(SliderGrabbed);
        interactable.onSelectExit.RemoveListener(SliderReleased);
        interactable.onHoverEnter.RemoveListener(SliderHovered);
        interactable.onHoverExit.RemoveListener(SliderUnHovered);
    }
}
