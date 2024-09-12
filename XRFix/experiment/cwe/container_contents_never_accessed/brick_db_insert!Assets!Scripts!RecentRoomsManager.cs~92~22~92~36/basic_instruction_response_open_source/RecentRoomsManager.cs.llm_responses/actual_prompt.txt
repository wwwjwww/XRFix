using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System.IO;
using TMPro;

public class RecentRoomsManager : MonoBehaviour
{
    public UserSettings userSettings;
    public SessionManager SessionManager;

    public GameObject noSavedRoomsObject;
    public GameObject savedRoomsObject;

    public GameObject recentRoomPrefab;
    public GameObject loadingPage;

    private bool _initializedSavedRoomsList;
    private TextInfo _textInfo;

    private const int NumberOfRoomsToLoad = 40;

    public GameObject[] savedRooms;
    public TextMeshProUGUI[] recentRoomsRoomCodes;
    public TextMeshProUGUI[] recentRoomsNames;
    public TextMeshProUGUI[] recentRoomsBrickCounts;

    public PagedScroll pagedScroll;

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    

    private void OnEnable()
    {
        _textInfo = new CultureInfo("en-US", false).TextInfo;
        RenderSavedRoomsList();
    }

    private string[] FindSaves() {
        if(!Directory.Exists($"{(Application.isEditor ? Application.dataPath : Application.persistentDataPath)}/saves/"))
            Directory.CreateDirectory($"{(Application.isEditor ? Application.dataPath : Application.persistentDataPath)}/saves/");
        
        return Directory.GetFiles($"{(Application.isEditor ? Application.dataPath : Application.persistentDataPath)}/saves/")
            .Where(file => file.EndsWith(".bricks")).ToArray();
    }

    private void RenderNoSavedRooms()
    {
        noSavedRoomsObject.SetActive(true);
        savedRoomsObject.SetActive(false);
        pagedScroll.DisableButtons();
    }

    private void RenderSavedRoomsList()
    {
        noSavedRoomsObject.SetActive(false);
        savedRoomsObject.SetActive(true);

        InitializeSavedRoomsList();
        _initializedSavedRoomsList = true;
    }

    private void InitializeSavedRoomsList()
    {
        string[] rooms = FindSaves();

        if(rooms.Length <= 0) {
            RenderNoSavedRooms();
            return;
        }

        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         List<string> roomNamesToLoad = new List<string>();

        // FIXED CODE:
