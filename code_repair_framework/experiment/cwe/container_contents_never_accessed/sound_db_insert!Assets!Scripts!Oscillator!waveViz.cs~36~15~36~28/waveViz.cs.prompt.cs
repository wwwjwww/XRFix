













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

  [DllImport("SoundStageNative")]
  public static extern void ProcessWaveTexture(float[] buffer, int length, System.IntPtr pixels, byte Ra, byte Ga, byte Ba, byte Rb, byte Gb, byte Bb,
    int period, int waveheight, int wavewidth, ref int lastWaveH, ref int curWaveW);

  // BUG: Container contents are never accessed
  // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
  //   List<float> bufferDrawList;

  // FIXED CODE:
