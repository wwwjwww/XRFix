    void Start () {
		vec = new Vector2(0,0);
	}
    void Update (){
        waverend.material.mainTextureOffset = vec;
        waverend.material.mainTextureOffset.x = (float)curWaveW / wavewidth;
        waverend.material.mainTextureOffset.y = 0;
    }
