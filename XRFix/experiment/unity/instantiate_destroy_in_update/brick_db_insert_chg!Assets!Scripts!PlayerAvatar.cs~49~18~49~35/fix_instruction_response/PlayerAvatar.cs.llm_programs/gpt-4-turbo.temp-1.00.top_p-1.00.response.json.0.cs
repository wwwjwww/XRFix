using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    // ... (Other class members remain unchanged)

    private bool create = true;
    private GameObject poolObject;

    public void Start() {
        source = FindObjectOfType<LocalRigData>();

        // ... (Remaining Start() method code remains unchanged) 

        // Object pooling setup
        poolObject = Instantiate(gobj5);
        poolObject.SetActive(false);
    }

    public void Update() {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit && create) {
            poolObject.SetActive(true);
            timer = 0;
            instantiate_gobj = true;
        } else if (instantiate_gobj && timer >= timeLimit) {
            poolObject.SetActive(false);
            timer = 0;
            instantiate_gobj = false;
        }

        if (!isLocal) return;

        // ... (Remaining Update() method code remains unchanged)
    }

    // ... (Rest of the PlayerAvatar class remains unchanged)
}
