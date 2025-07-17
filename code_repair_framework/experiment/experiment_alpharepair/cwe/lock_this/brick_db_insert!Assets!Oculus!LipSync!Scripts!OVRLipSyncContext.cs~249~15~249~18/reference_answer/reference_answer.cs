     private readonly object lockObject = new object();

     public void ProcessAudioSamplesRaw(short[] data, int channels)
     {

             lock (lockObject)
             {
                 if (Context == 0 || OVRLipSync.IsInitialized() != OVRLipSync.Result.Success)
                 {
                         return;
                 }
                 var frame = this.Frame;
                 OVRLipSync.ProcessFrame(Context, data, frame, channels == 2);
             }
     }