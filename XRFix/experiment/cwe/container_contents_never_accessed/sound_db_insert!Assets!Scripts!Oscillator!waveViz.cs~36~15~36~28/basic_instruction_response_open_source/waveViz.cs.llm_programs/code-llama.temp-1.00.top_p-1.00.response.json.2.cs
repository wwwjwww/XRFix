













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
    int period, int waveheight, int wavewidth, ref int lastWaveH, ref int curWaveW);

  // BUG: Container contents are never accessed
  // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
  //   List<float> bufferDrawList;

  // FIXED CODE:
  // FIXED CODE:
  
  // Remove the unused variable and its initialization
  public float bufferDrawList = new float();

  // Add the following function to create and initialize the wave texture
  void CreateWaveTexture()
  {
    tex = new Texture2D(wavewidth, waveheight);
    tex.filterMode = FilterMode.Point;
    wavepixels = new Color32[wavewidth * waveheight];
    for (int i = 0; i < wavewidth * waveheight; i++)
    {
      wavepixels[i] = waveBG;
    }
    tex.SetPixels32(wavepixels);
    tex.Apply();
  }

  // Insert a call to the CreateWaveTexture() function in the Start() function
  void Start()
  {
    CreateWaveTexture();
    // ...
  }

  // Replace the following line with a call to the CreateWaveTexture() function
  // tex = new Texture2D(wavewidth, waveheight);

  // Add the following function to update the wave texture
  void UpdateWaveTexture()
  {
    // ...
    tex.SetPixels32(wavepixels);
    tex.Apply();
    curWaveW++;
    if (curWaveW >= wavewidth)
    {
      curWaveW = 0;
      lastWaveH++;
      if (lastWaveH >= waveheight)
      {
        lastWaveH = 0;
      }
    }
  }

  // Replace the following line with a call to the UpdateWaveTexture() function
  // tex.SetPixels32(wavepixels);
  // tex.Apply();

  // Change the parameter type of the DllImport attribute to Int32 instead of int
  [DllImport("SoundStageNative")]
  int period, int waveheight, int wavewidth, ref int lastWaveH, ref int curWaveW);

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

  void Update() {
    tex.SetPixels32(wavepixels);
    tex.Apply(false);
    waverend.material.mainTextureOffset = new Vector2((float)curWaveW / wavewidth, 0);
  }
}
