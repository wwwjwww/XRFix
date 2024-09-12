    void Start () {
		vec = new Vector2(0,0);
	}
    void Update (){
        vec.x = entry.Value;
        vec.y = entry.Value + .01f;
        _deviceInterface.SpawnTimelineEvent(entry.Key, vec);
    }
