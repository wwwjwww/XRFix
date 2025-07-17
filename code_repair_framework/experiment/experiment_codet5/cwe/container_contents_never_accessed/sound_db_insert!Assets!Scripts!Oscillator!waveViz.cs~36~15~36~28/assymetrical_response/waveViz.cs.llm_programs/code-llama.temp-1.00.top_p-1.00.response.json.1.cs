













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

  /* BUG: Container contents are never accessed
  * MESSAGE: A collection or map whose contents are never queried or accessed is useless.
  *   List<float> bufferDrawList;

  * Remove or Commented-out the collection if it is no longer needed
  * FIXED CODE:
  */
  /// <summary>
/// Waveform visualization script for Unity Engine.
/// </summary>

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

  [DllImport("SoundStageNative")] // This line imports the SoundStageNative DLL, which is not used in the code provided.
    int period, int waveheight, int wavewidth, ref int lastWaveH, ref int curWaveW);

  private void Start() {
    // Initialize the waveform visualization texture and set up the renderer.
    tex = new Texture2D(wavewidth, waveheight, TextureFormat.RGBA32, false);
    waverend = GetComponent<Renderer>();
    waverend.material.SetTexture("_MainTex", tex);
    wavepixels = new Color32[wavewidth * waveheight];
    lastWaveH = waveheight - 1;
    curWaveW = 0;
  }

  private void Update() {
    float period_float = (float)period;
    float wavewidth_float = (float)wavewidth;
    float waveheight_float = (float)waveheight;

    // Generate a sine wave with the specified period, height, and width.
    for (int i = 0; i < wavewidth; i++) {
      float xPos = i / wavewidth_float;
      float yPos = Mathf.Sin(xPos * 2 * Mathf.PI * period_float) * 0.5f + 0.5f;
      wavepixels[i + curWaveW] = new Color32(waveBG.r, waveBG.g, waveBG.b, (byte)(waveBG.a * yPos));
    }

    // Update the renderer with the new waveform visualization texture.
    tex.SetPixels32(wavepixels);
    tex.Apply();
    waverend.material.SetTexture("_MainTex", tex);
  }
}


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
