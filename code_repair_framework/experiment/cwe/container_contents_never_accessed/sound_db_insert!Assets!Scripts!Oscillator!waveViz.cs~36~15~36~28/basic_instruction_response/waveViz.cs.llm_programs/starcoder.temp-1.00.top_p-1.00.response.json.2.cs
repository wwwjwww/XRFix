













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
  int curWaveH = 0;
int lastWaveW = 0;

// Wave texture
Texture2D waveTex;
// Wave material
Material waveMat;

void Start()
{
    // Create a new texture for the waves
    waveTex = new Texture2D(wavewidth, waveheight);
    waveMat = new Material(Shader.Find("Unlit/Texture"));
    waveMat.mainTexture = waveTex;

    // Set the wave properties
    waveMat.SetInt("_WaveWidth", wavewidth);
    waveMat.SetInt("_WaveHeight", waveheight);
    waveMat.SetInt("_Period", period);
}

void Update()
{
    // Get the wave buffer from the plugin
    int curWaveW;
    int lastWaveH;
    GetWaves(out curWaveW, out lastWaveH);

    // Copy the new wave data into the texture
    waveTex.SetPixels32(lastWaveW, 0, curWaveW - lastWaveW, waveheight, wavepixels);
    waveTex.Apply();

    // Set the new wave properties
    waveMat.SetInt("_WaveWidth", curWaveW);
    waveMat.SetInt("_LastWaveHeight", lastWaveH);

    // Set the wave material on the renderer
    waverend.material = waveMat;

    // Update the last wave width
    lastWaveW = curWaveW;
}

void GetWaves(out int curWaveW, out int lastWaveH)
{
    // Get a buffer for the waves

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
