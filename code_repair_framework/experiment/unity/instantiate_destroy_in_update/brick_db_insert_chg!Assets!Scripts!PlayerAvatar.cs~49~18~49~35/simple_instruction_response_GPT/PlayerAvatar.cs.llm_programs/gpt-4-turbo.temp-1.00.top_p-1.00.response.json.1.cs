using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    // ... (other code remains unchanged)

    private void Start()
    {
        // ... (other code in Start remains unchanged)
        
        InvokeRepeating(nameof(InstantiateAndDestroyObject), timeLimit, timeLimit);
    }

    // ... (other code remains unchanged)

    private void Update()
    {
        // Removed Instantiate and Destroy from here
        if (!isLocal) return;
        
        rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
        leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
        head.SetPositionAndRotation(source.head.position, source.head.rotation);
        transform.localScale = localRig.transform.localScale;
    }

    private void InstantiateAndDestroyObject()
    {
        if (!instantiate_gobj)
        {
            a5 = Instantiate(gobj5);
            instantiate_gobj = true;
        }
        else
        {
            Destroy(a5);
            instantiate_gobj = false;
        }
    }
}
}
