//Here're the buggy code lines from /Assets/Scripts/PlayerUIController.cs:

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    //...

    protected GameObject gobj5;
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    //...

    void Start()
    {
        //...
        hideUIbutton.performed += ctx => { OnHidePressed(); };

        InputAction hideUIbutton = new InputAction(binding: "<Keyboard>/escape");
        hideUIbutton.performed += ctx => { OnHidePressed(); };
        hideUIbutton.Enable();
    }

    private void OnHidePressed()
    {
        if (isDisplayingAlbums)
        {
            albumUI.albumUI.SetActive(false);
            isDisplayingAlbums = false;
            return;
        }
        if (isDisplayingPhotos)
        {
            photosUI.photosUI.SetActive(false);
            isDisplayingPhotos = false;
            return;
        }
    }

    private void Update()
    {
        if (gobj5 == null)
            gobj5 = Instantiate(Resources.Load<GameObject>("Prefabs/Prefab5"));

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

        //...
    }

    //...


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
