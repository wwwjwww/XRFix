using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected Transform a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    private GameObject a5_parent;

    private void Awake()
    {
        a5_parent = new GameObject("a5_parent");
        a5 = a5_parent.transform;
    }

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

    public void Update()
    {
        if (!instantiate_gobj && timer >= timeLimit)
        {
            var go = Instantiate(gobj5);
            a5 = go.transform;
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a5.gameObject);
            timer = 0;
            instantiate_gobj = false;
        }
    }

    public void LateUpdate()
    {
        if(!isLocal) return;
        
       rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
       leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
       head.SetPositionAndRotation(source.head.position, source.head.rotation);
       transform.localScale = localRig.transform.localScale;
    }

}
