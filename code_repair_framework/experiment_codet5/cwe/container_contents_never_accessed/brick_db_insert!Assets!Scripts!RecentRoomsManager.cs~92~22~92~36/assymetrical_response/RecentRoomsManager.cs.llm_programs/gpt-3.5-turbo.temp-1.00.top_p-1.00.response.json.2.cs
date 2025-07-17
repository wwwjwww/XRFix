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

        /* BUG: Container contents are never accessed
        * MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        *         List<string> roomNamesToLoad = new List<string>();

        * Remove or Commented-out the collection if it is no longer needed
        * FIXED CODE:
        */
        List<string> roomNamesToLoad = new List<string>();
        roomNamesToLoad.AddRange(rooms.Select(Path.GetFileNameWithoutExtension));
        roomNamesToLoad = roomNamesToLoad.GetRange(0, Mathf.Min(roomNamesToLoad.Count, NumberOfRoomsToLoad));

        savedRooms = new GameObject[roomNamesToLoad.Count];
        recentRoomsRoomCodes = new TextMeshProUGUI[roomNamesToLoad.Count];
        recentRoomsNames = new TextMeshProUGUI[roomNamesToLoad.Count];
        recentRoomsBrickCounts = new TextMeshProUGUI[roomNamesToLoad.Count];

        for (int i = 0; i < roomNamesToLoad.Count; i++)
        {
            savedRooms[i] = Instantiate(recentRoomPrefab, savedRoomsObject.transform);
            savedRooms[i].GetComponentInChildren<CustomButton>().onClick.AddListener(() => LoadRoom(roomNamesToLoad[i]));
            recentRoomsRoomCodes[i] = savedRooms[i].transform.Find("RoomCode").GetComponent<TextMeshProUGUI>();
            recentRoomsNames[i] = savedRooms[i].transform.Find("RoomName").GetComponent<TextMeshProUGUI>();
            recentRoomsBrickCounts[i] = savedRooms[i].transform.Find("BrickCount").GetComponent<TextMeshProUGUI>();

            string roomName = roomNamesToLoad[i];
            recentRoomsRoomCodes[i].text = roomName;
            recentRoomsNames[i].text = _textInfo.ToTitleCase(roomName.Replace("_", " "));
            recentRoomsBrickCounts[i].text = GetBrickCount(rooms[i]);
        }

        pagedScroll.InitializePagination(savedRooms.Length);
        pagedScroll.EnableButtons();
    }

    private void LoadRoom(string roomName)
    {
        SessionManager.DecodeSavedRoom(roomName);
    }

    private string GetBrickCount(string roomPath)
    {
        int brickCount = File.ReadAllLines(roomPath + ".bricks").Length;
        return brickCount + " Bricks";
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
