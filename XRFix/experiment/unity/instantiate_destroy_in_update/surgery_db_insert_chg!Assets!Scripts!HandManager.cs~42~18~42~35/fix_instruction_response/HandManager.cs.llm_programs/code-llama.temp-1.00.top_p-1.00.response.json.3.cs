using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandManager : MonoBehaviour
{
    public XRControllerHandler leftController;
    public XRControllerHandler rightController;

    void Start()
    {
        XRControllerHandler[] controllers = FindObjectsOfType<XRControllerHandler>();

        foreach (XRControllerHandler controller in controllers)
        {
            if (controller.controllerNode == XRControllerNode.Left)
            {
                leftController = controller;
            }
            else if (controller.controllerNode == XRControllerNode.Right)
            {
                rightController = controller;
            }
        }
    }

    void Update()
    {
        if (leftController.isSelecting)
        {
            // FIXED CODE: Use a RaycastHit variable to store the result of the raycast.
            RaycastHit hit;

            // FIXED CODE: Use a Ray object to create a ray that originates from the camera and points in the direction of the pointer.
            Ray ray = Camera.main.ScreenPointToRay(leftController.pointer.position);

            // FIXED CODE: Use the Raycast function to see if any objects are hit by the ray.
            if (Physics.Raycast(ray, out hit, 30f))
            {
                // FIXED CODE: Do not set the target of the left controller to be the GameObject hit by the raycast.
                // Instead, check if the GameObject hit by the raycast has the "Item" component, and if it does, set the target of the left controller to be that GameObject.
                if (hit.collider.gameObject.GetComponent<Item>())
                {
                    leftController.target = hit.collider.gameObject.GetComponent<Item>();
                }
            }
        }

        // FIXED CODE: Use a RaycastHit variable to store the result of the raycast.
        RaycastHit hit;

        // FIXED CODE: Use a Ray object to create a ray that originates from the camera and points in the direction of the pointer.
        Ray ray = Camera.main.ScreenPointToRay(rightController.pointer.position);

        // FIXED CODE: Use the Raycast function to see if any objects are hit by the ray.
        if (Physics.Raycast(ray, out hit, 30f))
        {
            // FIXED CODE: Do not set the target of the right controller to be the GameObject hit by the raycast.
            // Instead, check if the GameObject hit by the raycast has the "Item" component, and if it does, set the target of the right controller to be that GameObject.
            if (hit.collider.gameObject.GetComponent<Item>())
            {
                rightController.target = hit.collider.gameObject.GetComponent<Item>();
            }
        }
    }
}

