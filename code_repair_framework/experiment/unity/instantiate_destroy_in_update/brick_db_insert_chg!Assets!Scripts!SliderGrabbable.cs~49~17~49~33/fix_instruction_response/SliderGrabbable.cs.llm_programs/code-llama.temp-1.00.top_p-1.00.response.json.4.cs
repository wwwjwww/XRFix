

using UnityEngine;

public class SliderGrabbable : MonoBehaviour
{
    public bool isGrabbable = true;
    public float grabDistance = 3.0f;

    private Rigidbody rb;
    private Grabber grabbedBy;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isGrabbable && Input.GetMouseButtonDown(0))
        {
            Grabber closestGrabber = null;
            float closestDistance = grabDistance;

            foreach (Grabber grabber in FindObjectsOfType<Grabber>())
            {
                float distance = Vector3.Distance(grabber.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestGrabber = grabber;
                    closestDistance = distance;
                }
            }

            if (closestGrabber != null)
            {
                grabbedBy = closestGrabber;
                grabbedBy.grabObject(this);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Grabber>())
        {
            isGrabbable = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Grabber>())
        {
            isGrabbable = true;
        }
    }

    void OnDisable()
    {
        if (grabbedBy != null)
        {
            grabbedBy.dropObject(gameObject);
            grabbedBy = null;
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
