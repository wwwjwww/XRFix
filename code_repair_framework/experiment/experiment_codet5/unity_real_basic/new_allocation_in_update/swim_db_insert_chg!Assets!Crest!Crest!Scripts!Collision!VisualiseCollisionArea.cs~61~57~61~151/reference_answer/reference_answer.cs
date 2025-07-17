    void Start () {
		vec = new Vector3(0,0,0);
	}
    void Update (){
        _samplePositions[j * s_steps + i] = vec;
        _samplePositions[j * s_steps + i].x = ((i + 0.5f) - s_steps / 2f) * s_radius;
        _samplePositions[j * s_steps + i].y = 0f;
        _samplePositions[j * s_steps + i].z = ((j + 0.5f) - s_steps / 2f) * s_radius);
    }
