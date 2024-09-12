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

    float timeLimit = 5f;
    float timer = 0f;
    bool instantiate_gobj = false;
    GameObject gobj3;
    GameObject a3;

    private void Start()
    {
        formatModal.SetActive(false);
        filterModal.SetActive(false);
        videoFormatSelectHint.SetActive(false);
        OnFormatSelect(Utility.PhotoTypes.RectangularMono);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a3 = Instantiate(gobj3);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a3);
            timer = 0;
            instantiate_gobj = false;
        }

        if (displayVideoOnNextFrame)
        {
            playerUIController.HideLoader();
            photoDisplayer.DisplayVideo();
            displayVideoOnNextFrame = false;
        }
    }

    public void Free(GameObject obj3)
    {
       Destroy(obj3);
    }

}

