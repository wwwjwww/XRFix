/* Here're the buggy code lines from /Assets/Scripts/PlayerAvatar.cs:*/
﻿using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;



    private bool? _local = null;
    public bool isLocal {
        get {
            if(_local == null)
                _local = this == AvatarManager.GetInstance().LocalAvatar;

            return _local.Value;
        }
    }

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public GameObject nameLabel;
    public GameObject face;

    private GameObject localRig;

    public void Start() {
        source = FindObjectOfType<LocalRigData>();

        if(!isLocal) return;
        
        nameLabel.SetActive(false);
        face.SetActive(false);
        localRig = GameObject.Find("Player Controllers/VR Rig");
    }

/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     public void Update() {
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit)
*         {
*             a5 = Instantiate(gobj5);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit )
*         {
*             Destroy(a5);
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
*         if(!isLocal) return;
*         
*         rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
*         leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
*         head.SetPositionAndRotation(source.head.position, source.head.rotation);
*         transform.localScale = localRig.transform.localScale;
*     }

* you can try to build an object pool before Update() method has been called.
* FIXED CODE:
*/

private ObjectPool objectPool;

public void Start()
{
    source = FindObjectOfType<LocalRigData>();

    if (!isLocal) return;

    nameLabel.SetActive(false);
    face.SetActive(false);
    localRig = GameObject.Find("Player Controllers/VR Rig");

    objectPool = new ObjectPool(gobj5);
}

public void Update()
{
    if (!isLocal) return;

    rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
    leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
    head.SetPositionAndRotation(source.head.position, source.head.rotation);
    transform.localScale = localRig.transform.localScale;

    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a5 = objectPool.GetObject();
        timer = 0;
        instantiate_gobj = true;
    }
    else if (instantiate_gobj && timer >= timeLimit)
    {
        objectPool.ReturnObject(a5);
        timer = 0;
        instantiate_gobj = false;
    }
}

}
