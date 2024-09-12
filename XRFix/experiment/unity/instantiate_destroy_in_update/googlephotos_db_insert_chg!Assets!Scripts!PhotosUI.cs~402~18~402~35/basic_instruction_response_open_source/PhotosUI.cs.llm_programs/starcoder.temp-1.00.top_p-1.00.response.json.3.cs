//Here're the buggy code lines from /Assets/Scripts/PhotosUI.cs:
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public class PhotosUI : MonoBehaviour
{
    public enum FilterMode
    {
        Unfiltered,
        SphericalPhotosOnly,
        StereoPhotosOnly,
        VideosOnly
    };
    public float paddingLoadMoreButton;
    public int numColumnsPerRow;
    public Sprite iconRectangularMono;
    public Sprite iconRectangularStereo;
    public Sprite iconSphericalMono;
    public Sprite iconSphericalStereo;

    public GameObject photosUI;
    public Image formatButtonIcon;
    public GameObject formatModal;
    public GameObject filterModal;
    public RectTransform scrollViewContent;
    public RectTransform loadMoreButton;
    public TextMeshProUGUI albumTitle;
    public GameObject photoEntryPrefab;
    public PhotoDisplayer photoDisplayer;
    public GameObject videoFormatSelectHint;
    [System.NonSerialized]
    public PlayerUIController playerUIController;
    [System.NonSerialized]
    FilterMode filterMode = FilterMode.Unfiltered;

    float entryDimension;
    bool isShowingLibrary = false;
    string displayedAlbumId = null;
    FilterMode displayedFilterMode = FilterMode.Unfiltered;
    List<string> instantiatedPhotoKeys = new List<string>();
    Dictionary<string, PhotoUIEntry> instantiatedEntries = new Dictionary<string, PhotoUIEntry>();
    PhotoUIEntry selectedEntry;
    bool displayVideoOnNextFrame = false;

    public bool PhotoHasBeenSelected => selectedEntry!= null;

    protected GameObject gobj3;
    protected GameObject a3;

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;


    private void Start()
    {
        formatModal.SetActive(false);
        filterModal.SetActive(false);
        videoFormatSelectHint.SetActive(false);
        OnFormatSelect(Utility.PhotoTypes.RectangularMono);
    }

    public void RefreshDisplay()
    {
        if (isShowingLibrary) DisplayLibrary(playerUIController.photosDataManager.data);
        else if (displayedAlbumId!= null) DisplayAlbum(playerUIController.photosDataManager.data, displayedAlbumId);
    }

    public void DisplayLibrary(PhotosDataStore data)
    {
        if (!isShowingLibrary || displayedAlbumId!= null)
        {
            DestroyAllEntries();
        }
        isShowingLibrary = true;
        albumTitle.text = PlayerUIController.ALL_PHOTOS_TEXT;
        displayedAlbumId = null;
        StartCoroutine(DisplayPhotos(data.library.mediaItems, data.library.hasMoreMediaItemsToLoad));
    }

    public void DisplayAlbum(PhotosDataStore data, string albumKey)
    {
        Album album = data.albums[albumKey];
        if(isShowingLibrary || displayedAlbumId!= album.id)
        {
            DestroyAllEntries();
        }
        isShowingLibrary = false;
        albumTitle.text = album.title;
        displayedAlbumId = album.id;
        StartCoroutine(DisplayPhotos(album.mediaItems, album.hasMoreMediaItemsToLoad));
    }

    IEnumerator DisplayPhotos(Dictionary<string, MediaItem> mediaItems, bool hasMoreMediaItemsToLoad)
    {
        yield return new WaitForEndOfFrame(); 
        entryDimension = scrollViewContent.rect.width / numColumnsPerRow;

        if(filterMode!= displayedFilterMode)
        {
            displayedFilterMode = filterMode;
            DestroyAllEntries();
        }

        int pos = 0;
        for (int i = 0; i < mediaItems.Count; i++)
        {
            var kvp = mediaItems.ElementAt(i);
            MediaItem mediaItem = kvp.Value;

            if (!ShouldBeShown(mediaItem)) continue;
            
            if (!instantiatedPhotoKeys.Contains(kvp.Key))
            {
                instantiatedPhotoKeys.Add(kvp.Key);

                int row = pos / numColumnsPerRow;
                int col = pos % numColumnsPerRow;

                GameObject newEntry = Instantiate(photoEntryPrefab, scrollViewContent);
                RectTransform rt = newEntry.GetComponent<RectTransform>();

                rt.SetInsetAndSizeFromParentEdge(
                    RectTransform.Edge.Top,
                    entryDimension * row,
                    entryDimension
                );
                rt.SetInsetAndSizeFromParentEdge(
                    RectTransform.Edge.Left,
                    entryDimension * col,
                    entryDimension
                );

                PhotoUIEntry photoUIEntry = newEntry.GetComponent<PhotoUIEntry>();
                instantiatedEntries[kvp.Key] = photoUIEntry;
                string imageUrl = null;
                if (mediaItem.IsPhoto) imageUrl = mediaItem.baseUrl + "=w500-h500-c";
                else if (mediaItem.IsVideo) imageUrl = mediaItem.baseUrl + "=w500-h500-c";
                if (imageUrl!= null) StartCoroutine(photoUIEntry.DownloadThumbnail(mediaItem, playerUIController.photosDataManager));
                photoUIEntry.button.onClick.AddListener(() => OnSelectPhoto(mediaItem));
            }

            pos++;
        }

        int numRows = (int)Mathf.Ceil((float)pos / numColumnsPerRow);
        scrollViewContent.sizeDelta = new Vector2(
            0,
            entryDimension * numRows +
            paddingLoadMoreButton * 2 +
            loadMoreButton.rect.height
        );
        loadMoreButton.SetInsetAndSizeFromParentEdge(
            RectTransform.Edge.Top,
            entryDimension * numRows + paddingLoadMoreButton,
            loadMoreButton.rect.height
        );
        loadMoreButton.gameObject.SetActive(hasMoreMediaItemsToLoad);
    }

    public bool ShouldBeShown(MediaItem mediaItem)
    {
        if (filterMode == FilterMode.Unfiltered)
        {
            return true;
        }
        else if (filterMode == FilterMode.SphericalPhotosOnly)
        {
            return mediaItem.projection == "equirectangular";
        }
        else if (filterMode == FilterMode.StereoPhotosOnly)
        {
            return true;
        }
        else if(filterMode == FilterMode.VideosOnly)
        {
            return mediaItem.IsVideo;
        }
        return false;
    }

    public void DestroyAllEntries()
    {
        foreach (var kvp in instantiatedEntries)
        {
            Destroy(kvp.Value.gameObject);
        }
        instantiatedPhotoKeys.Clear();
        instantiatedEntries.Clear();
        scrollViewContent.anchoredPosition = Vector2.zero;
        selectedEntry = null;
    }

    public void LoadMore()
    {
        if (isShowingLibrary)
        {
            playerUIController.LoadLibraryMediaItems();
        }
        else
        {
            playerUIController.LoadAlbumMediaItems(displayedAlbumId);
        }
    }

    void OnSelectPhoto(MediaItem mediaItem)
    {
        if (!instantiatedEntries.ContainsKey(mediaItem.id)) return;
        videoFormatSelectHint.SetActive(false);
        PhotoUIEntry entry = instantiatedEntries[mediaItem.id];
        if (selectedEntry!= null)
        {
            selectedEntry.SetSelected(false);
        }
        if (selectedEntry == entry)
        {
            selectedEntry = null;
        }
        else
        {
            entry.SetSelected(true);
            selectedEntry = entry;
        }

        if(selectedEntry == null)
        {
            photoDisplayer.StopDisplaying();
        }
        else if (mediaItem.IsPhoto)
        {
            playerUIController.DisplayLoader();
            if (mediaItem.downloadedImageTexture == null)
            {
                StartCoroutine(playerUIController.photosDataManager.DownloadPhotoContent(mediaItem, AfterPhotoDownloaded, OnDownloadProgressChange));
            }
            else
            {
                AfterPhotoDownloaded(mediaItem);
            }
        }
        else if (mediaItem.IsVideo)
        {
            playerUIController.DisplayLoader();
            if (mediaItem.downloadedVideoFilePath == null)
            {
                StartCoroutine(playerUIController.photosDataManager.DownloadVideoContent(mediaItem, AfterVideoDownloaded, OnDownloadProgressChange));
            }
            else
            {
                photoDisplayer.CurrentMediaItem = mediaItem;
                displayVideoOnNextFrame = true;
                if (photoDisplayer.PhotoType == Utility.PhotoTypes.RectangularMono)
                    videoFormatSelectHint.SetActive(true);
            }
        }
    }

    public void ClearSelection()
    {
        videoFormatSelectHint.SetActive(false);

        if (selectedEntry!= null)
        {
            selectedEntry.SetSelected(false);
            selectedEntry = null;
        }
    }

    void OnDownloadProgressChange(float progress)
    {
        playerUIController.DisplayLoader(progress);
    }

    void AfterPhotoDownloaded(MediaItem mediaItem)
    {
        photoDisplayer.CurrentMediaItem = null;
        SwitchPhotoTypeBasedOnMetadata(mediaItem);
        playerUIController.HideLoader();
        photoDisplayer.CurrentMediaItem = mediaItem;
        photoDisplayer.DisplayPhoto();
    }

    void AfterVideoDownloaded(MediaItem mediaItem)
    {
        photoDisplayer.CurrentMediaItem = null;
        
        playerUIController.HideLoader();
        photoDisplayer.CurrentMediaItem = mediaItem;
        photoDisplayer.DisplayVideo();
        if(photoDisplayer.PhotoType == Utility.PhotoTypes.RectangularMono)
            videoFormatSelectHint.SetActive(true);
    

    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
