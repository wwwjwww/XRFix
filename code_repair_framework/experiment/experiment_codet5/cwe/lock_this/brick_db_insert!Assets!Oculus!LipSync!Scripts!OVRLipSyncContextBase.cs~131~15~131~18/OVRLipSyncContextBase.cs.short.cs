using UnityEngine;

///     void OnDestroy()
//     {
// 
        // BUG: Locking the 'this' object in a lock statement
        // MESSAGE: It is bad practice to lock the 'this' object because it might be locked elsewhere.
        //         lock (this)
        //         {
        //             if (context != 0)
        //             {
        //                 if (OVRLipSync.DestroyContext(context) != OVRLipSync.Result.Success)
        //                 {
        //                     Debug.LogError("OVRLipSyncContextBase.OnDestroy ERROR: Could not delete" +
        //                         " Phoneme context.");
        //                 }
        //             }
        //         }
        //     }

        // Create a private readonly Object which is used exclusively for locking. This ensures that no other classes can use the same lock.
        // FIXED CODE:
