using UnityEngine;
using System.Collections;

///   void Update() {
    // BUG: Transform object of Rigidbody in Update() methods
    // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
    //     rb10.transform.Rotate(0, 30, 0);
    // 
    //     if (!active) return;
    // 
    //     source.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
    //     GenerateTex();
    //     tex.SetPixels32(texpixels);
    //     tex.Apply(false);
    //   }

    // FIXED CODE:
