using System.Collections;
using System.Collections.Generic;
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
        if (loaderProgressBar != null)
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
    {
        loader.SetActive(false);
        albumUI.albumUI.SetActive(true);
        photosUI.photosUI.SetActive(false);
        isDisplayingAlbums = true;
        isDisplayingPhotos = false;
        albumUI.DisplayAlbums(photosDataManager.data);
        photosUI.photoDisplayer.StopDisplaying();
        photosUI.ClearSelection();
        photosUI.OnFilterModeSelect(PhotosUI.FilterMode.Unfiltered);
        photosUI.photoDisplayer.onCurrentMediaItemChange.AddListener(OnCurrentMediaItemChanged);
    }

    public void DisplayPhotosFromLibrary()
    {
        loader.SetActive(false);
        albumUI.albumUI.SetActive(false);
        photosUI.photosUI.SetActive(true);
        isDisplayingAlbums = false;
        isDisplayingPhotos = true;
        photosUI.DisplayLibrary(photosDataManager.data);
    }

    public void DisplayPhotosFromAlbum(string albumKey)
    {
        loader.SetActive(false);
        albumUI.albumUI.SetActive(false);
        photosUI.photosUI.SetActive(true);
        isDisplayingAlbums = false;
        isDisplayingPhotos = true;
        photosUI.DisplayAlbum(photosDataManager.data, albumKey);
    }

    public void LoadAlbums()
    {
        DisplayLoader();
        StartCoroutine(LoadAlbumsCoroutine());
    }

    private IEnumerator LoadAlbumsCoroutine()
    {
        yield return new WaitForSeconds(1f); // Simulating async operation
        photosDataManager.FetchNextPageOfAlbumData();
        displayAlbumsOnNextFrame = true;
    }

    public void LoadLibraryMediaItems()
    {
        DisplayLoader();
        StartCoroutine(LoadLibraryMediaItemsCoroutine());
    }

    private IEnumerator LoadLibraryMediaItemsCoroutine()
    {
        yield return new WaitForSeconds(1f); // Simulating async operation
        photosDataManager.FetchNextPageOfLibraryMediaItems();
        displayLibraryOnNextFrame = true;
    }

    public void LoadAlbumMediaItems(string albumKey)
    {
        DisplayLoader();
        StartCoroutine(LoadAlbumMediaItemsCoroutine(albumKey));
    }

    private IEnumerator LoadAlbumMediaItemsCoroutine(string albumKey)
    {
        yield return new WaitForSeconds(1f); // Simulating async operation
        photosDataManager.FetchNextPageOfMediaItemsInAlbum(albumKey);
        albumToDisplayOnNextFrame = albumKey;
    }

    public void OnCurrentMediaItemChanged()
    {
        hideUIHintText.SetActive(isDisplayingPhotos && photosUI.photoDisplayer.CurrentMediaItem != null);
    }

    public void SignOut()
    {
        AuthenticationManager.Instance.SignOut();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a5 = Instantiate(gobj5);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            ReleaseInstance(a5);
            timer = 0;
            instantiate_gobj = false;
        }

        if (displayAlbumsOnNextFrame)
        {
            DisplayAlbumUI();
            displayAlbumsOnNextFrame = false;
        }
        else if (displayLibraryOnNextFrame)
        {
            DisplayPhotosFromLibrary();
            displayLibraryOnNextFrame = false;
        }
        else if (albumToDisplayOnNextFrame != null)
        {
            DisplayPhotosFromAlbum(albumToDisplayOnNextFrame);
            albumToDisplayOnNextFrame = null;
        }
    }

    private void ReleaseInstance(GameObject obj)
    {
        Destroy(obj);
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


