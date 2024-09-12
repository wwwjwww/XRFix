// FIXED CODE:

using UnityEngine;

public class Slice : MonoBehaviour
{
    public GameObject pivot;
    public GameObject handle;
    public GameObject toSlice;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == handle)
        {
            toSlice.transform.SetParent(pivot.transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == handle)
        {
            toSlice.transform.SetParent(null, true);
        }
    }
}
