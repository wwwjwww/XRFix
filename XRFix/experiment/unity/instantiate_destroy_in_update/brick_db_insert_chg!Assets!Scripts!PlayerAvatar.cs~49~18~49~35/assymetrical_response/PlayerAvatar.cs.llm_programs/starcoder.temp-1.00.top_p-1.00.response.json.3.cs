using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;


    private static object poolLock = new object();
    private static ObjectPool<GameObject> pool;

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

        // Initialize object pool
        if (pool == null) {
            lock (poolLock) {
                if (pool == null) {
                    pool = new ObjectPool<GameObject>(
                        () => Instantiate(gobj5),
                        o => o.SetActive(false)
                    );
                }
            }
        }
    }

    public void Update()
    {
        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a5 = pool.Get();
            time
}
