Here's your updated code:

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public const string ALL_PHOTOS_TEXT = "All photos";

    public InputAction hideUIbutton;
    public GameObject content;
    public PhotosDataManager photosDataManager;
    public GameObject loader;
    public ProgressBar loaderProgressBar;
    public GameObject hideUIHintText;
    public AlbumUI albumUI;
    public PhotosUI photosUI;

    bool displayAlbumsOnNextFrame;
    bool displayLibraryOnNextFrame;
    string albumToDisplayOnNextFrame;
    bool isDisplayingAlbums;
    bool isDisplayingPhotos;

    protected GameObject gobj5;
    protected GameObject a5;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;


    void Start()
    {
        hideUIbutton.performed += ctx => { OnHidePressed(); };
        albumUI.playerUIController = this;
        photosUI.playerUIController = this;
        albumUI.albumUI.SetActive(false);
        photosUI.photosUI.SetActive(false);
        hideUIHintText.SetActive(false);
        LoadAlbums();
    }

    public void DisplayLoader(float progress = 0)
    {
        loader.SetActive(true);
        if (loaderProgressBar!= null)
        {
            loaderProgressBar.gameObject.SetActive(progress > 0);
            loaderProgressBar.SetProgress(progress);
        }
    }
    public void HideLoader()
    {
        loader.SetActive(false);
    }

    public void DisplayAlbumUI()
    

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
