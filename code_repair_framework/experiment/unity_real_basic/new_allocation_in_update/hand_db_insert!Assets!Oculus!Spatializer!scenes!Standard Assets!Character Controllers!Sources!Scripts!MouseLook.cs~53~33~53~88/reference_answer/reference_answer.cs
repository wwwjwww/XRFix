    void Start () {
		vec = new Vector3(0,0,0);
	}
    void Update (){
        transform.localEulerAngles = vec;
        transform.localEulerAngles.x = -rotationY;
        transform.localEulerAngles.y = transform.localEulerAngles.y;
        transform.localEulerAngles.z = 0;
    }
