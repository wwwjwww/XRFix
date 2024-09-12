// Here're the buggy code lines from /Assets/Scripts/PlayerAvatar.cs:

using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;


public bool? local {
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

    if(!local) return;
    
    nameLabel.SetActive(false);
    face.SetActive(false);
    localRig = GameObject.Find("Player Controllers/VR Rig");
}

public void Update() {
    timer+=Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        a5 = Object.Instantiate(gobj5);
        timer = 0;
        instantiate_gobj = true;
    }
    if (instantiate_gobj && timer >= timeLimit )
    {
        Object.Destroy(a5);
        timer = 0;
        instantiate_gobj = false;
    }

    if(!local) return;

    rightHand.Se
}
