//Here're the buggy code lines from /Assets/Scripts/PlayerAvatar.cs:

using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    private Queue<GameObject> objectPool = new Queue<GameObject>();

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

        gobj5 = new GameObject("Cube");
        gobj5.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        objectPool.Enqueue(gobj5);
    }

    public void Update() {
        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a5 = GetObjectFromPool();
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit )
        {
            returnObjectToPool(a5);
            timer = 0;
            instantiate_gobj = false;
        }

        if(!isLocal) return;
        
        rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
        leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
        head.SetPositionAndRotation(source.head
}
