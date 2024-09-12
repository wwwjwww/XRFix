using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    //...

    public void Start() 
    {
        //...
        InvokeRepeating(nameof(ToggleGameObject), timeLimit, timeLimit);
    }

    private void ToggleGameObject()
    {
        if (!a5)
        {
            a5 = Instantiate(gobj5);
        }
        else
        {
            Destroy(a5);
            a5 = null;
        }
    }

    public void Update() 
    {
        if(!isLocal) return;

        rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
        leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
        head.SetPositionAndRotation(source.head.position, source.head.rotation);
        transform.localScale = localRig.transform.localScale;
    }
}
}
