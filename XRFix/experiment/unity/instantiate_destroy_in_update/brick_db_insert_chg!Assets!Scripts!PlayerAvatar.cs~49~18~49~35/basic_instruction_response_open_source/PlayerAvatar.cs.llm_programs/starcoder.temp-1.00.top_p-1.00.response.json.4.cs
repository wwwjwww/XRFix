using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private LocalRigData source;

    protected GameObject gobj5; 
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    //added new coroutine
    private IEnumerator Start()
    {
        source = FindObjectOfType<LocalRigData>();

        if (!isLocal) yield break;
        
        nameLabel.SetActive(false);
        face.SetActive(false);
        localRig = GameObject.Find("Player Controllers/VR Rig");

        while (true)
        {
            //added new yield return
            yield return new WaitForSeconds(1f);
            Debug.Log("Coroutine running!");
        }
    }


    // Update is called once per frame
    public void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a5 = Instantiate(gobj5);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a5);
            timer = 0;
            instantiate_gobj = false;
        }

        if (!isLocal
}
