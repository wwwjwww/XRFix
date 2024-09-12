













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
  void Update() {
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

    // Move this function in FixedUpdate() methods.
    // FIXED CODE:

    El cambio climático tiene un impacto ambiental significativo en todo el mundo. Algunos de los efectos más comunes incluyen:

1.  Calor excesivo: La mayoría de las temperaturas más altos se han registrado en las áreas más altas del planeta, provocando una mayor frecuencia y intensidad de sequías y inundaciones. Además, el calor excesivo a menudo está asociado con tormentas más fuertes y destructivas.

2.  Riesgos de incendios forestales: Las explosiones de fuego en bosques y matorrales son una consecuencia común de la sequía y la deforestación causadas por el cambio climático. Estas explosiones pueden ser extremadamente destructivas, incluso provocando la muerte de muchos personas.

3.  Alteración de la cadena alimentaria: El cambio climático puede afectar la disponibilidad y la qualidad de los alimentos, lo que puede tener un impacto en la salud y el bienestar de las personas. Además, la sequía y las inundaciones pueden hacer que sea más difícil cultivar y distribuir alimentos.

4.  Acumulación de glaciers y ríos: La deglaciación y la subida de los niveles del mar pueden tener un impacto en la seguridad alimentaria, ya que la mayoría de los pueblos y ciudades están ubicados cerca de los ríos y los glaciares.

5.  Escasez de agua: La escasez de agua puede tener un impacto en la seguridad alimentaria, la salud y el bienestar de las personas, ya que a menudo se asocia con una mayor frecuencia de sequías y inundaciones.

6.  Afectación a la biodiversidad: El cambio climático puede afectar a la vida silvestre y a la biodiversidad en general. Esto puede incluir una disminución de la

  public void Flush() {
    for (int i = 0; i < wavewidth; i++) {
      for (int i2 = 0; i2 < waveheight; i2++) {
        wavepixels[i2 * wavewidth + i] = waveBG;
      }
    }
    if (curTape != null) Destroy(curTape.gameObject);
    sampleBuffer = new float[Mathf.RoundToInt((float)(biggestBeats * biggestPeriod * .25f * AudioSettings.outputSampleRate))];
    tex.SetPixels32(wavepixels);
    tex.Apply(false);
  }


  public void Save() {
    if (bufferToWav.instance.savingInProgress) return;
    string audioFilename = masterControl.instance.SaveDir + System.IO.Path.DirectorySeparatorChar + "Samples" + System.IO.Path.DirectorySeparatorChar +
         "Recordings" + System.IO.Path.DirectorySeparatorChar +
        string.Format("{0:MM-dd_hh-mm-ss-tt}.wav",
        DateTime.Now);
    bufferToWav.instance.Save(audioFilename, sampleBuffer, 2, virtualBufferLength, saveText, this);
  }

  string curfilename = "";
  public override void updateTape(string s) {
    s = System.IO.Path.GetFileNameWithoutExtension(s);
    curfilename = s;
    if (curTape != null) Destroy(curTape.gameObject);
    curTape = (Instantiate(tapePrefab, tapeHolder, false) as GameObject).GetComponent<tape>();
    curTape.Setup(s, sampleManager.instance.sampleDictionary["Recordings"][s]);
  }

  void createNewTape() {
    Vector3 p;
    Quaternion q;
    curTape.getOrigTrans(out p, out q);
    curTape.masterObj = null;
    curTape = (Instantiate(tapePrefab, tapeHolder, false) as GameObject).GetComponent<tape>();

    curTape.transform.localPosition = p;
    curTape.Setup(curfilename, sampleManager.instance.sampleDictionary["Recordings"][curfilename]);
  }

  public void Back() {
    curBufferIndex = 0;
    resetScrub = true;
  }

  float recordStartPos = 0;
  public void Record() {
    playing = true;
    recording = true;
  }

  public void Stop() {
    playing = false;
    resetScrub = true;
    _deviceInterface.buttons[1].phantomHit(playing);

    if (!playing && recording) {
      recording = false;
      _deviceInterface.buttons[0].phantomHit(recording);
    }
  }

  double lastIncomingDspTime = -1;
  float[] oldBuffer;
  bool playingLastFrame = false;
  public override void processBuffer(float[] buffer, double dspTime, int channels) {
    int tempColumnMult = columnMult;

    if (lastIncomingDspTime == dspTime) {
      for (int i = 0; i < buffer.Length; i++) {
        buffer[i] = oldBuffer[i];
      }
      return;
    }
    lastIncomingDspTime = dspTime;

    oldBuffer = new float[buffer.Length];
    float[] recBuffer = new float[buffer.Length];
    float[] playBuffer = new float[buffer.Length];

    if (_deviceInterface.recordTrigger.signal != null) _deviceInterface.recordTrigger.signal.processBuffer(recBuffer, dspTime, channels);
    if (_deviceInterface.playTrigger.signal != null) _deviceInterface.playTrigger.signal.processBuffer(playBuffer, dspTime, channels);

    if (incoming != null) incoming.processBuffer(buffer, dspTime, channels);

    Color curWaveLine = waveLine;
    Color curWaveBG = waveBG;
    for (int i = 0; i < buffer.Length; i += channels) {
      if (_deviceInterface.playTrigger.signal != null) {
        if (playBuffer[i] > lastPlaySig[1] && lastPlaySig[1] <= lastPlaySig[0]) {
          playing = true;
          curBufferIndex = 0;
          _deviceInterface.buttons[1].phantomHit(playing);
        }

        lastPlaySig[0] = lastPlaySig[1];
        lastPlaySig[1] = playBuffer[i];
      }

      if (_deviceInterface.recordTrigger.signal != null) {
        if (recBuffer[i] > lastRecSig[1] && lastRecSig[1] <= lastRecSig[0]) {
          recording = !recording;
          _deviceInterface.buttons[0].phantomHit(recording);

          if (recording && !playing) {
            playing = true;
            curBufferIndex = 0;
            _deviceInterface.buttons[1].phantomHit(playing);
          }
        }

        lastRecSig[0] = lastRecSig[1];
        lastRecSig[1] = recBuffer[i];
      }

      if (playing) {
        oldBuffer[i] = buffer[i] += sampleBuffer[curBufferIndex];
        oldBuffer[i + 1] = buffer[i + 1] += sampleBuffer[curBufferIndex + 1];
      }

      if (recording) {
        sampleBuffer[curBufferIndex] = buffer[i];
        sampleBuffer[curBufferIndex + 1] = buffer[i + 1];
        curWaveLine = waveLineRec;
        curWaveBG = waveBGRec;
      } else {
        curWaveLine = waveLine;
        curWaveBG = waveBG;
      }

      if (playing) {
        int centerH = waveheight / 2;
        if (curBufferIndex % tempColumnMult == 0) {
          curWaveW = curBufferIndex / tempColumnMult;

          if (curWaveW < wavewidth) {
            int curH = Mathf.FloorToInt((waveheight - 1) * .5f * Mathf.Clamp01(Mathf.Abs(sampleBuffer[curBufferIndex])));

            for (int i2 = 0; i2 < centerH; i2++) {
              if (i2 < curH) wavepixels[(centerH - i2) * wavewidth + curWaveW] = wavepixels[(centerH + i2) * wavewidth + curWaveW] = curWaveLine;
              else wavepixels[(centerH - i2) * wavewidth + curWaveW] = wavepixels[(centerH + i2) * wavewidth + curWaveW] = waveBG;
            }
          }
        }

        curBufferIndex = (curBufferIndex + 2);
        if (curBufferIndex >= virtualBufferLength) {
          if (recording) {
            recording = !recording;
            _deviceInterface.buttons[0].phantomHit(recording);
          }
          curBufferIndex = 0;

          if (_deviceInterface.playTrigger.signal != null) Stop();

          if (recordRequested) {
            recording = true;
            _deviceInterface.playClick = true;
            _deviceInterface.buttons[0].phantomHit(recording);
            recordRequested = false;
            _deviceInterface.recordCountdown = false;
          }
        }

        if (recordRequested) {
          _deviceInterface.recCountdownRemaining = Mathf.CeilToInt((virtualBufferLength - curBufferIndex) / (2 * (float)_sampleRateOverride));
          if (lastremaining != _deviceInterface.recCountdownRemaining) {
            _deviceInterface.playClick = true;
            lastremaining = _deviceInterface.recCountdownRemaining;
          }
        }
      }
    }

    samplePos = (float)curBufferIndex / virtualBufferLength;
  }

  bool recordRequested = false;
  public void requestRecord(bool on) {
    recordRequested = on;
    lastremaining = Mathf.CeilToInt((virtualBufferLength - curBufferIndex) / (2 * (float)_sampleRateOverride));
  }

  int lastremaining = 0;
}