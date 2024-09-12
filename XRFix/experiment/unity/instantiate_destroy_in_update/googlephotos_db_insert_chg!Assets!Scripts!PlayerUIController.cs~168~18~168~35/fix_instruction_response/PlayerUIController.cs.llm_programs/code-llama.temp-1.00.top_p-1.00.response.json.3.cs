using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public GameObject healthBar;
    public GameObject shieldBar;
    public GameObject armorBar;
    
    void Start()
    {
        healthBar.SetActive(false);
        shieldBar.SetActive(false);
        armorBar.SetActive(false);
    }
    
    void Update()
    {
        if (healthBar.activeSelf)
        {
            healthBar.SetActive(false);
        }
        if (shieldBar.activeSelf)
        {
            shieldBar.SetActive(false);
        }
        if (armorBar.activeSelf)
        {
            armorBar.SetActive(false);
        }
    }
}

    public void ReleaseInstance(GameObject instance5)
    {
        Destroy(instance5);
    }

    void OnHidePressed() {
        if(content.activeSelf && isDisplayingPhotos && photosUI.photoDisplayer.CurrentMediaItem != null)
        {
            content.SetActive(false);
        } else
        {
            content.SetActive(true);
        }
    }

    void OnEnable()
    {
        hideUIbutton.Enable();
    }
    void OnDisable()
    {
        hideUIbutton.Disable();
    }
}
