    void Start () {
        vec = new Vector3(0,0,0);
	}
    void Update (){
        manager.headPoseRelativeOffsetRotation.x = emulatedRoll;
        manager.headPoseRelativeOffsetRotation.y = emulatedYaw;
        manager.headPoseRelativeOffsetRotation.z = emulatedPitch;
    }
