













using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class waveViz : MonoBehaviour {
  Texture2D tex;
  public Renderer waverend;
  int wavewidth = 512;
  int waveheight = 64;
  public int period = 512;
  Color32[] wavepixels;
  int curWaveW = 0;
  int lastWaveH = 0;
  public Color32 waveBG = Color.black;
  public Color32 waveLine = Color.white;

  [DllImport("SoundStageNative")]
  public static extern void ProcessWaveTexture(float[] buffer, int length, System.IntPtr pixels, byte Ra, byte Ga, byte Ba, byte Rb, byte Gb, byte Bb,
    int period, int waveheight, int wavewidth, ref int lastWaveH, ref int curWaveW);

  List<float> bufferDrawList;

  GCHandle m_WavePixelsHandle;

  void Awake() {
    bufferDrawList = new List<float>();
    tex = new Texture2D(wavewidth, waveheight, TextureFormat.RGBA32, false);
    wavepixels = new Color32[wavewidth * waveheight];

    for (int i = 0; i < wavewidth; i++) {
      for (int i2 = 0; i2 < waveheight; i2++) {
        wavepixels[i2 * wavewidth + i] = waveBG;
      }
    }

    m_WavePixelsHandle = GCHandle.Alloc(wavepixels, GCHandleType.Pinned);
  }

  void OnDestroy() {
    m_WavePixelsHandle.Free();
  }

  void Start() {
    tex.SetPixels32(wavepixels);
    tex.Apply(false);
    waverend.material.mainTexture = tex;
  }

  public void UpdateViz(float[] buffer) {
    ProcessWaveTexture(buffer, buffer.Length, m_WavePixelsHandle.AddrOfPinnedObject(), waveLine.r, waveLine.g, waveLine.b, waveBG.r, waveBG.g, waveBG.b, period, waveheight, wavewidth, ref lastWaveH, ref curWaveW);
  }

///   void Update() {
//     tex.SetPixels32(wavepixels);
//     tex.Apply(false);
    // BUG: Using New() allocation in Update() method.
    // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
    //     waverend.material.mainTextureOffset = new Vector2((float)curWaveW / wavewidth, 0);
    //   }

    // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
    // FIXED CODE:


  private Vector2 waveOffset = Vector2.zero;

  void Update() {
    tex.SetPixels32(wavepixels);
    tex.Apply(false);
    waveOffset.x = (float)curWaveW / wavewidth;
    waverend.material.mainTextureOffset = waveOffset;
  }


}
