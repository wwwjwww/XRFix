  private List<GameObject> planeObjectPool = new List<GameObject>();

  void Initialize (){
    for (int i = 0; i < m_AllPlanes.Count; i++)
    {
        GvrAudio audioSource = planeObjectPool[i];
        audioSource.SetActive(true);
    }
  }

  private void ShutdownSource () {
    if (id >= 0) {
      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Id, -1.0f);

      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.ZeroOutput, 1.0f);
      audioSource.spatialize = false;
      audioSource[id].SetActive(false);
      id = -1;
    }
  }
