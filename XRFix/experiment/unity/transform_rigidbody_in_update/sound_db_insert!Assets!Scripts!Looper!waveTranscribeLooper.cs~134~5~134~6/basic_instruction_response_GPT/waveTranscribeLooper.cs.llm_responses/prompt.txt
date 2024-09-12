













using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class waveTranscribeLooper : signalGenerator {
  public GameObject tapePrefab;
  public Transform tapeHolder;
  public tape curTape;
  public bool recording = false;
  public bool playing = false;

  public TextMesh saveText;

  int maxDuration = 300;

  float[] sampleBuffer;
  int curBufferIndex = 0;

  int virtualBufferLength = 0;

  public Transform scrubTransform;
  Vector2 scrubRange = new Vector2(.4f, -.4f);

  public signalGenerator incoming;
  looperDeviceInterface _deviceInterface;

  
  Texture2D tex;
  public Renderer waverend;
  int wavewidth = 1024;
  int waveheight = 512;
  public int period = 512;
  Color32[] wavepixels;
  int curWaveW = 0;
  int lastWaveH = 0;
  public Color32 waveBG = Color.black;
  public Color32 waveBGRec = Color.black;
  public Color32 waveLine = Color.white;
  public Color32 waveLineRec = Color.white;
  int columnMult = 1;
  double _sampleRateOverride;

  float[] lastRecSig, lastPlaySig, lastBackSig;

  float biggestBeats = 1;
  double biggestPeriod = 0.0625;

  protected Rigidbody rb;



  public override void Awake() {
    base.Awake();
    _deviceInterface = GetComponent<looperDeviceInterface>();
    

    lastbeatperiod = _deviceInterface.period;
    virtualBufferLength = Mathf.RoundToInt((float)(_deviceInterface.period * .25f * AudioSettings.outputSampleRate));
    sampleBuffer = new float[virtualBufferLength];

    _sampleRateOverride = AudioSettings.outputSampleRate;
    tex = new Texture2D(wavewidth, waveheight, TextureFormat.RGBA32, false);
    wavepixels = new Color32[wavewidth * waveheight];
    waverend.material.mainTexture = tex;

    lastRecSig = new float[] { 0, 0 };
    lastPlaySig = new float[] { 0, 0 };
  }

  double lastbeatperiod = 0;

  public void updateDuration(float beats, double beatperiod) {
    virtualBufferLength = Mathf.RoundToInt((float)(beats * beatperiod * .25f * AudioSettings.outputSampleRate));
    if (beatperiod > biggestPeriod || beats > biggestBeats) {
      if (biggestBeats < beats) biggestBeats = beats;
      if (biggestPeriod < beatperiod) biggestPeriod = beatperiod;
      Array.Resize<float>(ref sampleBuffer, Mathf.RoundToInt((float)(biggestBeats * biggestPeriod * .25f * AudioSettings.outputSampleRate)));
    }

    columnMult = Mathf.CeilToInt((float)virtualBufferLength / (wavewidth - 1));
    recalcTex();
    if (!playing) {
      tex.SetPixels32(wavepixels);
      tex.Apply(false);
    }
  }

  void recalcTex() {
    int centerH = waveheight / 2;

    for (int i = 0; i < wavewidth; i++) {
      if (columnMult * i < virtualBufferLength) {
        int curH = Mathf.FloorToInt((waveheight - 1) * .5f * Mathf.Clamp01(Mathf.Abs(sampleBuffer[columnMult * i])));

        for (int i2 = 0; i2 < centerH; i2++) {
          if (i2 < curH) wavepixels[(centerH - i2) * wavewidth + i] = wavepixels[(centerH + i2) * wavewidth + i] = waveLine;
          else wavepixels[(centerH - i2) * wavewidth + i] = wavepixels[(centerH + i2) * wavewidth + i] = waveBG;
        }
      }
    }
  }

  void Start() {
    Flush();
    tex.SetPixels32(wavepixels);
    tex.Apply(false);
    scrubTransform.localRotation = Quaternion.identity;
    columnMult = Mathf.CeilToInt((float)virtualBufferLength / (wavewidth - 1));
  }

  void scrubReset() {
    resetScrub = false;
    scrubTransform.localRotation = Quaternion.identity;
  }

  bool resetScrub = false;
  float samplePos = 0;
///   void Update() {
    // BUG: Transform object of Rigidbody in Update() methods
    // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
    //     rb.transform.Translate(0, 0, Time.deltaTime);
    //     
    //     if (resetScrub) scrubReset();
    // 
    //     scrubTransform.gameObject.SetActive(playing || _deviceInterface.recordCountdown || _deviceInterface.playCountdown);
    //     if (!playing) {
    //       return;
    //     }
    // 
    //     tex.SetPixels32(wavepixels);
    //     tex.Apply(false);
    // 
    //     if (curTape != null) {
    //       if (curTape.inDeck()) {
    //         createNewTape();
    //       }
    //     }
    // 
    //     scrubTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, samplePos));
    //   }

    // FIXED CODE:
