    void Start () {
		vec = new Vector3(0,0,0);
	}
    void Update (){
        manager.transform.localScale.x = data.RootScale;
        manager.transform.localScale.y = data.RootScale;
        manager.transform.localScale.z = data.RootScale;
    }
