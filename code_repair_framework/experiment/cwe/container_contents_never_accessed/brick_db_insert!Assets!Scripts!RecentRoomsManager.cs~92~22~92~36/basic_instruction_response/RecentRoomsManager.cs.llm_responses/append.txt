
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
