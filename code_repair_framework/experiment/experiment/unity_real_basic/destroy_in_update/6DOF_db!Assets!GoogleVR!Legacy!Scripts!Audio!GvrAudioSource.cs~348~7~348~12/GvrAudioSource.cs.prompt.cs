// BUG: Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
//  void Update () {
//
//    if (!occlusionEnabled) {
//      currentOcclusion = 0.0f;
//    } else if (Time.time >= nextOcclusionUpdate) {
//      nextOcclusionUpdate = Time.time + GvrAudio.occlusionDetectionInterval;
//      currentOcclusion = GvrAudio.ComputeOcclusion(transform);
//    }
//
//    if (!isPlaying && !isPaused) {
//      Stop();
//    } else {
//      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Gain,
//                                      GvrAudio.ConvertAmplitudeFromDb(gainDb));
//      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.MinDistance,
//                                      sourceMinDistance);
//      GvrAudio.UpdateAudioSource(id, this, currentOcclusion);
//    }
//  }
