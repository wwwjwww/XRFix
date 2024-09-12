     private readonly object lockObject = new object();

     void OnDestroy()
     {

         lock (lockObject)
         {
             if (context != 0)
             {
                 if (OVRLipSync.DestroyContext(context) != OVRLipSync.Result.Success)
                 {
                     Debug.LogError("OVRLipSyncContextBase.OnDestroy ERROR: Could not delete" +
                         " Phoneme context.");
                 }
             }
         }
     }