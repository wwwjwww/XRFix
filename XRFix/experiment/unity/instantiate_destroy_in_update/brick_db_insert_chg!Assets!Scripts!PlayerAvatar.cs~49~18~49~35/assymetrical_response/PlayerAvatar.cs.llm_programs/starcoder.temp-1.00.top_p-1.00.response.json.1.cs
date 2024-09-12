using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5;
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    public GameObject prefab;

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public GameObject nameLabel;
    public GameObject face;

    private GameObject localRig;

    private GameObject instance;

    public void Start()
    {
        source = FindObjectOfType<LocalRigData>();

        if (!isLocal) return;

        nameLabel.SetActive(false);
        face.SetActive(false);
        localRig = GameObject.Find("Player Controllers/VR Rig");
    }

    public void Update()
    {
        if (!isLocal) return;

        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            instance = Instantiate(prefab);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(instance);
            timer = 0;
            instantiate_gobj = false;
        }

        rightHand.SetPositionAndRotation(source.rightHand.position, source.rightHand.rotation);
        leftHand.SetPositionAndRotation(source.leftHand.position, source.leftHand.rotation);
        head.SetPositionAndRotation(source.head.position, sourc
}
