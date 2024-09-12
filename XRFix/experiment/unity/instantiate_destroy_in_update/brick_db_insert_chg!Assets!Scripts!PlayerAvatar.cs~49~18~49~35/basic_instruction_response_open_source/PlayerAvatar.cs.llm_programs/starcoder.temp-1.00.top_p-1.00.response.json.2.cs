using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    private void Update()
    {
        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a5 = Instantiate(gobj5);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit )
        {
            Destroy(a5);
            timer = 0;
            instantiate_gobj = false;
        }

        if(!isLocal) return;
        
        rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
        leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
        head.SetPositionAndRotation(source.head.position, source.head.rotation);
        transform.localScale = localRig.transform.localScale;
    }

}
}
