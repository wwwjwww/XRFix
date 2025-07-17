     private readonly object lockObject = new object();

     void Awake()
     {

         if (!audioSource)
         {
             audioSource = GetComponent<AudioSource>();
         }


         lock (lockObject)
         {
             if (context == 0)
             {
                   if (OVRLipSync.CreateContext(ref context, provider, 0, enableAcceleration)
                             != OVRLipSync.Result.Success)
                   {
                        Debug.LogError("OVRLipSyncContextBase.Start ERROR: Could not create" +
                                 " Phoneme context.");
                             return;
                   }
             }
         }
     }