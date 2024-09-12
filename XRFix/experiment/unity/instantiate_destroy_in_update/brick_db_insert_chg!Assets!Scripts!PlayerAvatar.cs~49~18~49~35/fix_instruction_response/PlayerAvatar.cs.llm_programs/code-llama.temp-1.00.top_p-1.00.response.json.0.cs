using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    public GameObject Avatar { get; set; }

    void Start()
    {
        // Added check to ensure that the Avatar game object is not null
        if (Avatar != null)
        {
            // Attack the target
            Avatar.GetComponent<Enemy>().Attack();
        }
    }
}

