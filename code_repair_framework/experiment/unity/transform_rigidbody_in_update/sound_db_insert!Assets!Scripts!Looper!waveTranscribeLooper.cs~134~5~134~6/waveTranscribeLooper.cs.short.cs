using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

///   void Update() {
    // BUG: Transform object of Rigidbody in Update() methods
    // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
    //     rb.transform.Translate(0, 0, Time.deltaTime);
    //     
    //     if (resetScrub) scrubReset();
    // 
    //     scrubTransform.gameObject.SetActive(playing || _deviceInterface.recordCountdown || _deviceInterface.playCountdown);
    //     if (!playing) {
    //       return;
    //     }
    // 
    //     tex.SetPixels32(wavepixels);
    //     tex.Apply(false);
    // 
    //     if (curTape != null) {
    //       if (curTape.inDeck()) {
    //         createNewTape();
    //       }
    //     }
    // 
    //     scrubTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, samplePos));
    //   }

    // FIXED CODE:
