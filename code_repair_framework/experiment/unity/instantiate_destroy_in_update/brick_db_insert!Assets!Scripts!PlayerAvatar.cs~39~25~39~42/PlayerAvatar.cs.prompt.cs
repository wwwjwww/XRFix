using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5;


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

    public void Update() {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         GameObject a5 = Instantiate(gobj5);
        //         Destroy(a5);
        // 
        //         if(!isLocal) return;
        //         
        //         rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
        //         leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
        //         head.SetPositionAndRotation(source.head.position, source.head.rotation);
        //         transform.localScale = localRig.transform.localScale;
        //     }

        // FIXED VERSION:
