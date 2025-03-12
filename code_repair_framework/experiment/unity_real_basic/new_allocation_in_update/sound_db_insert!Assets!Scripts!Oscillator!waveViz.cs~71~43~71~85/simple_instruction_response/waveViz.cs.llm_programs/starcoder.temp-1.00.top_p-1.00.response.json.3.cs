













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

  void Update() {
    tex.SetPixels32(wavepixels);
    tex.Apply(false);
    
    
using System;
using UnityEngine;

public class WaveViz : MonoBehaviour
{
    public Renderer waveRenderer;
    public int waveWidth = 512;
    public int waveHeight = 64;
    public int period = 512;
    Color32[] wavePixels;
    int curWaveW = 0;
    int lastWaveH = 0;
    public Color32 waveBg = Color.black;
    public Color32 waveLine = Color.white;

    [DllImport("SoundStageNative")]
    private static extern void ProcessWaveTexture(float[] buffer, int length, IntPtr pixels, byte redA, byte greenA, byte blueA, byte redB, byte greenB, byte blueB, int period, int waveHeight, int waveWidth, ref int lastWaveH, ref int curWaveW);

    private List<float> bufferDrawList = new List<float>();
    private Texture2D waveTexture;
    private GCHandle wavePixelsHandle;

    private void Awake()
    {
        wavePixels = new Color32[waveWidth * waveHeight];

        for (int i = 0; i < waveWidth; i++)
        {
            for (int i2 = 0; i2 < waveHeight; i2++)
            {
                wavePixels[i2 * waveWidth + i] = waveBg;
            }
        }

        waveTexture = new Texture2D(waveWidth, waveHeight, TextureFormat.RGBA32, false);
        waveTexture.SetPixels32(wavePixels);
        waveTexture.Apply(false);

        waveRenderer.material.mainTexture = waveTexture;
        wavePixelsHandle = GCHandle.Alloc(wavePixels, GCHandleType.Pinned);
    }

    private void OnDestroy()
    {
        wavePixelsHandle.Free();
        Destroy(waveTexture);
    }

    private void Start()
    {
        ProcessWaveTexture(new float[waveWidth], waveWidth, wavePixelsHandle.AddrOfPinnedObject(), waveLine.r, waveLine.g, waveLine.b, waveBg.r, waveBg.g, waveBg.b, period, waveHeight, waveWidth, ref lastWaveH, ref curWaveW);
    }

    public void UpdateViz(float[] buffer)
    {
        ProcessWaveTexture(buffer, buffer.Length, wavePixelsHandle.AddrOfPinnedObject(), waveLine.r, waveLine.g, waveLine.b, waveBg.r, waveBg.g, waveBg.b, period, waveHeight, waveWidth, ref lastWave
  }
}
