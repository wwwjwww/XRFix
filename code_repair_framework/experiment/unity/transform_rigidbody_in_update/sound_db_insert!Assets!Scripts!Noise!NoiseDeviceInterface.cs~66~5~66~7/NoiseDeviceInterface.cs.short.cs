using UnityEngine;
using System.Collections;

///   void Update() {
    // BUG: Transform object of Rigidbody in Update() methods
    // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
    //     rb2.transform.Translate(0, 1, Time.deltaTime);
    //     
    //     if (gen.updated || output.near == null) {
    //       gen.updated = false;
    //       GenerateRandomTex();
    //       tex.SetPixels32(texpixels);
    //       tex.Apply(false);
    //     }
    // 
    //     gen.updatePercent(speedDial.percent);
    //   }

    // FIXED CODE:
