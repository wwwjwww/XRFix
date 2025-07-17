//  private void ShutdownSource () {
//    if (id >= 0) {
//      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Id, -1.0f);
//
//      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.ZeroOutput, 1.0f);
//      audioSource.spatialize = false;
//      GvrAudio.DestroyAudioSource(id);
//      id = -1;
//    }
//  }

// FIXED CODE:

// you can try to build an object pool before Update() method has been called.