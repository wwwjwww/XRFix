













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

  //Remove or Commented-out the collection if it is no longer needed
  // FIXED CODE:
  using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class waveViz : MonoBehaviour
{
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

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    // FIXED CODE: List<float> bufferDrawList = new List<float>();

    [DllImport("SoundStageNative")]
    int period, int waveheight, int wavewidth, ref int lastWaveH, ref int curWaveW);

    // Remove or Commented-out the collection if it is no longer needed
    // FIXED CODE: //bufferDrawList.Clear();

    private void Update()
    {
        if (tex == null)
            return;

        // BUG: Buffer is not updated as it does not exist in native code
        // MESSAGE: Buffer 'buffer' does not exist in the native code.
        // FIXED CODE: // UpdateBuffer(tex, wavewidth, waveheight);

        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        // FIXED CODE: // bufferDrawList;

        // FIXED CODE:
        //if (!bufferDrawList.Any())
        //{
        //    UpdateBuffer(tex, wavewidth, waveheight);
        //}
    }

    public void UpdateBuffer(Texture2D texture, int width, int height)
    {
        if (texture == null)
            return;

        // BUG: No update of buffer is done if the contents of the buffer are never queried or accessed.
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        // FIXED CODE:
        //wavepixels = texture.GetPixels32();

        // BUG: No update of buffer is done if the contents of the buffer are never queried or accessed.
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        // FIXED CODE:
        //curWaveW = 0;
        //lastWaveH = 0;
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
