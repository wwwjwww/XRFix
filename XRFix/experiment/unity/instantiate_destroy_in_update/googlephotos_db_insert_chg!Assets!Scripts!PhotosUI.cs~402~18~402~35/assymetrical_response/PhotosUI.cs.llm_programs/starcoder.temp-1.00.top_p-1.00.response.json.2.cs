using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

public class PhotosUI : MonoBehaviour
{
    // Object pool for recycling prefab game objects
    public GameObject photoEntryPrefab;
    public List<GameObject> photoEntryInstances = new List<GameObject>();

    // Other component references
    public RectTransform scrollViewContent;
    public RectTransform loadMoreButton;
    public TextMeshProUGUI albumTitle;
    public PhotoDisplayer photoDisplayer;

    // State variables
    public bool PhotoHasBeenSelected => selectedEntry!= null;
    protected PhotoUIEntry selectedEntry;

    // Settings
    public float paddingLoadMoreButton;
    public int numColumnsPerRow;

    // Enumerations for identifying photo types
    public enum FilterMode
    {
        Unfiltered,
        SphericalPhotosOnly,
        StereoPhotosOnly,
        VideosOnly
    };

    // Current filter mode for photo display
    private FilterMode filterMode = FilterMode.Unfiltered;

    // The following two variables keep track of the currently displayed album and its filter mode
    private string displayedAlbumId = null;
    private FilterMode displayedFilterMode = FilterMode.Unfiltered;

    // List of instantiated photo keys
    private List<string> instantiatedPhotoKeys = new List<string>();

    // Dictionary to map photo keys to their respective UI entry game objects
    private Dictionary<string, PhotoUIEntry> instantiatedEntries = new Dictionary<string, PhotoUIEntry>();

    // Variable to keep track of whether the library or an album is being displayed
    private bool isShowingLibrary = false;

    // Use this for initialization
    protected void Start()
    {
        // Initialize object pool
        for (int i = 0; i < photoEntryInstances.Capacity; i++)
        {
            GameObject obj = Instantiate(photoEntryPrefab);
            obj.SetActive(false);
            photoEntryInstances.Add(obj);
        }

        // Hook up event listener for when a new photo is selected
        photoDisplayer.OnPhotoDisplayed += OnPhotoDisplayed;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (displayVideoOnNextFrame)
        {
            playerUIController.HideLoader();
            photoDisplayer.DisplayVideo();
            displayVideoOnNextFrame = false;
        }
    }

    // Display the photo library
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

    // Display the photos in an album
    public void DisplayAlbum(PhotosDataStore data, string albumKey)
    {
        Album album = data.albums[albumKey];
        if (isShowingLibrary || displayedAlbumId!= album.id)
        {
            DestroyAllEntries();
        }
        isShowingLibrary = false;
        albumTitle.text = album.title;
        displayedAlbumId = album.id;
        StartCoroutine(DisplayPhotos(album.mediaItems, album.hasMoreMediaItemsToLoad));
    }

    // Display the next batch of photos
    IEnumerator DisplayPhotos(Dictionary<string, MediaItem> mediaItems, bool hasMoreMediaItemsToLoad)
    {
        entryDimension = scrollViewContent.rect.width / numColumnsPerRow;

        if (filterMode!= displayedFilterMode)
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

            // Check if the photo prefab is available in the object pool
            GameObject entryInstance = null;
            for (int j = 0; j < photoEntryInstances.Count; j++)
            {
                if (!photoEntryInstances[j].activeInHierarchy)
                {
                    entryInstance = photoEntryInstances[j];
                    break;
                }
            }
            if (entryInstance == null)
            {
                entryInstance = Instantiate(photoEntryPrefab);
            }
            entryInstance.SetActive(true);

            // Get the photo UI entry component for the given photo prefab
            PhotoUIEntry photoUIEntry = entryInstance.GetComponent<PhotoUIEntry>();

            // Store the photo key and the UI entry in the dictionary
            instantiatedPhotoKeys.Add(kvp.Key);
            instantiatedEntries[kvp.Key] = photoUIEntry;

            // Configure the photo UI entry
            int row = pos / numColumnsPerRow;
            int col = pos % numColumnsPerRow;
            RectTransform rt = entryInstance.GetComponent<RectTransform>();
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

            // Download the photo thumbnail if it hasn't been downloaded yet
            string imageUrl = null;
            if (mediaItem.IsPhoto) imageUrl = mediaItem.baseUrl + "=w500-h500-c";
            else if (mediaItem.IsVideo) imageUrl = mediaItem.baseUrl + "=w500-h500-c";
            if (imageUrl!= null) StartCoroutine(photoUIEntry.DownloadThumbnail(mediaItem, playerUIController.photosDataManager));

            // Hook up the event listener for when the photo is clicked
            photoUIEntry.button.onClick.AddListener(() => OnSelectPhoto(mediaItem));

            pos++;
        }

        // Destroy any unused photo prefabs in the object pool
        for (int i = 0; i < photoEntryInstances.Count; i++)
        {
            if (photoEntryInstances[i].activeInHierarchy) continue;
            Destroy(photoEntryInstances[i]);
            photoEntryInstances.RemoveAt(i);
            i--;
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

    // Check if a photo should be displayed based on the current filter mode
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
        else if (filterMode == FilterMode.VideosOnly)
        {
            return mediaItem.IsVideo;
        }
        return false;
    }

    // Destroy all the UI entries for the currently displayed album
    public void DestroyAllEntries()
    {
        foreach (var kvp in instantiatedEntries)
        {
            photoEntryInstances.Add(kvp.Value.gameObject);
            kvp.Value.button.onClick.RemoveAllListeners();
            kvp.Value.gameObject.SetActive(false);
        }
        instantiatedPhotoKeys.Clear();
        instantiatedEntries.Clear();
        scrollViewContent.anchoredPosition = Vector2.zero;
        selectedEntry = null;
    }

    // Load the next batch of photos
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

    // Event handler for when a new photo is displayed
    void OnPhotoDisplayed(MediaItem mediaItem)
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

        if (selectedEntry == null)
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
            

    public void Free(GameObject obj3)
    {
        Destroy(obj3);
    }
}
