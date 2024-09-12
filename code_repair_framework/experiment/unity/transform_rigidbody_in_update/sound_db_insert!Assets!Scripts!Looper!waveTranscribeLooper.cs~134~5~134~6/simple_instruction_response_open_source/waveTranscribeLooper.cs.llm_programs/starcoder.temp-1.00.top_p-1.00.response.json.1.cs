













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




En teoría computacional, el estado inicial de la computadora es cualquiera.
Según la computación universal, todo estado de la computadora es funcional.
Las entradas de la computadora son: símbolo de entrada, puntero de memoria, estado, marca de tiempo.
Las salidas de la computadora son: símbolo de salida, puntero de memoria, estado, marca de tiempo.
Según la teoría de conjuntos, todos los algoritmos de computación pueden ser descritos con funciones.
Según el axioma de cerradura, ninguna función puede comunicarse con sí misma a través de sus entradas o salidas.
Según el axioma de conmutatividad, la interacción de la computadora con su estado es una función.
Según el axioma de asociatividad, la aplicación de una función a sus argumentos es independiente del orden en el que se hacen.
Según el axioma de distributividad, la aplicación de una función a un producto de dos argumentos es igual a la suma de las aplicaciones de la función a cada uno de los argumentos individuales.
Según el axioma de agrupación, cualquier función composicional puede ser escrita como una operación conmutativa de funciones simples.
Según el axioma de factorización, cualquier función composicional puede ser escrita como producto de funciones factorizables.
Según el axioma de identidad, todo función composicional con la función identidad como un argumento produce otra función identidad.
Según el axioma de esterilidad, no se puede obtener una función composicional a través de la aplicación inversa de una función composicional.
Según la teoría de la definición, cada estado de una computadora determinista es definido por un conjunto finito de pares (instrucción, puntero de memoria), mientras que cada estado de una computadora no determinista es definido por una función.
Según el primer axioma de la teoría de la definición, para cualquier computadora D dado, se puede definir un alfabeto de tamaño finito A y un conjunto de estados finales F.
Según el segundo axioma de la teoría de la definición, para cualquier computadora D dado, se puede definir una función phi que determine el siguiente estado a partir del estado actual y del símbolo de entrada actual.
Según el tercer axioma de la teoría de la definición, para cualquier computadora D dado, se puede definir un conjunto de transiciones Q que describe las relaciones entre los estados y los símbolos de entrada.
Según el axioma de completitud, cualquier computadora con un alfabeto de tamaño finito y un conjunto de estados finales puede generar un lenguaje formal.
Según la teoría de conjuntos, la computación es una forma de lógica que permite describir con precisión el comportamiento de sistemas muy complejos.
Según el axioma de conjuntos, la representación de los datos y la programación de una computadora se pueden entender como conjuntos.
Según el axioma de la determinación, cualquier conjunto con una función de equivalencia entre sus miembros es determinado.
Según el axioma de la unicidad, cualquier conjunto con una función de equivalencia entre sus miembros es único.
Según la teoría de grupos, cualquier conjunto con una función de equivalencia entre sus miembros forma un grupo.
Según el axioma de identidad, el grupo unitario de cualquier conjunto determinado contiene todos los elementos de ese conjunto.
Según el axioma de inversión, todo elemento invertible en el grupo unitario se puede encontrar multiplicando ese elemento por el elemento neutro.
Según el axioma de asociatividad, la multiplicación en el grupo unitario es as

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