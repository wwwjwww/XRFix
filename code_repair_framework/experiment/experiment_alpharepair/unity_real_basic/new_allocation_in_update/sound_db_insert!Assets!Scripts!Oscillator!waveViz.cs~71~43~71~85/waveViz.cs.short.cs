using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

///   void Update() {
//     tex.SetPixels32(wavepixels);
//     tex.Apply(false);
    // BUG: Using New() allocation in Update() method.
    // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
    //     waverend.material.mainTextureOffset = new Vector2((float)curWaveW / wavewidth, 0);
    //   }

    // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
    // FIXED CODE:
