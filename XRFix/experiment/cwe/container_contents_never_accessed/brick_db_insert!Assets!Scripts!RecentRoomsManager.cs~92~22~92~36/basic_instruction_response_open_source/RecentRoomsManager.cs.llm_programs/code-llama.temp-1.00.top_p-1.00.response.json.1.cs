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
        The code contains a few errors and issues that need to be fixed. Here is the fixed code:

private void InitializeSavedRoomsList()
{
    string[] rooms = FindSaves();

    if (rooms.Length <= 0)
    {
        RenderNoSavedRooms();
        return;
    }

    List<string> roomNamesToLoad = new List<string>();
    for (int i = 0; i < Math.Min(rooms.Length, NumberOfRoomsToLoad); i++)
    {
        roomNamesToLoad.Add(rooms[i]);
    }

    for (int i = 0; i < Math.Min(savedRooms.Length, NumberOfRoomsToLoad); i++)
    {
        Destroy(savedRooms[i]);
    }

    for (int i = 0; i < Math.Min(roomNamesToLoad.Count, NumberOfRoomsToLoad); i++)
    {
        GameObject room = Instantiate(recentRoomPrefab, savedRoomsObject.transform);
        TMPro.TextMeshProUGUI roomCode = room.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI roomName = room.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI roomBrickCount = room.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>();

        roomCode.text = _textInfo.ToTitleCase(Path.GetFileNameWithoutExtension(roomNamesToLoad[i]));
        roomName.text = _textInfo.ToTitleCase(Path.GetFileNameWithoutExtension(roomNamesToLoad[i])) + "'s Brickery";
        roomBrickCount.text = "Brick count: " + (i + 1).ToString();

        savedRooms[i] = room;
    }

    if (roomNamesToLoad.Count <= 0)
    {
        RenderNoSavedRooms();
    }
    else
    {
        savedRoomsObject.SetActive(true);
        noSavedRoomsObject.SetActive(false);
    }
}

private string[] FindSaves()
{
    if (!Directory.Exists($"{(Application.isEditor ? Application.dataPath : Application.persistentDataPath)}/saves/"))
        Directory.CreateDirectory($"{(Application.isEditor ? Application.dataPath : Application.persistentDataPath)}/saves/");

    return Directory.GetFiles($"{(Application.isEditor ? Application.dataPath : Application.persistentDataPath)}/saves/")
        .Where(file => file.EndsWith(".bricks")).ToArray();
}

        for (int i = 0; i < Mathf.Min(rooms.Length, NumberOfRoomsToLoad); i++)
        {
            GameObject buttonObject = savedRooms[i];
            if (i < 4) buttonObject.SetActive(true);

            Button button = buttonObject.GetComponent<Button>();

            TextMeshProUGUI roomNameText = recentRoomsNames[i];

            string[] path = rooms[i].Split('/');
            roomNameText.text = path[path.Length - 1];

            int i1 = i;
            button.onClick.AddListener(new UnityAction(() => ButtonClicked(rooms[i1])));
        }

        pagedScroll.SetFixedElementCount(rooms.Length);
    }

    private void ButtonClicked(string roomName)
    {
        CallLoadEnum(roomName);
        gameObject.SetActive(false);
        loadingPage.SetActive(true);
    }

    private static void CallLoadEnum(string roomName) {
        SessionManager manager = SessionManager.GetInstance();
        manager.StartCoroutine(manager.session.LoadSave(roomName));
    }

    private string TitleCase(string roomName)
    {
        return _textInfo.ToTitleCase(roomName.ToLower());
    }

    private string FormatRoomNameAnyLenNoMono(string roomName)
    {
        if (roomName.Length <= 2)
            return roomName;

        if (roomName.Length <= 4)
            return $"{roomName.Substring(0, 2)} {roomName.Substring(2, (roomName.Length - 2))}";

        if(roomName.Length <= 6)
            return $"{roomName.Substring(0, 2)} {roomName.Substring(2, 2)} {roomName.Substring(4, (roomName.Length - 4))}";

        return $"{roomName.Substring(0, 2)} {roomName.Substring(2, 2)} {roomName.Substring(4, 2)} {roomName.Substring(6, (roomName.Length - 6))}";
    }
}
