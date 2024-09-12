using UnityEngine;
using UnityEngine.UI;

public class waveTranscribeLooper : MonoBehaviour
{
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
    looperDeviceInterface deviceInterface;

    [SerializeField] private RawImage display;
    [SerializeField] private int sampleRate = 44100;

    private float[] displayBuffer;
    private int displayBufferIndex;

    private double sampleRateOverride;
    private double lastBeatPeriod;
    private double beatMultiplier;
    private float biggestBeats;
    private double biggestPeriod;

    private Color32[] wavePixels;
    private Texture2D waveTexture;
    private Renderer waveRenderer;

    private Transform deviceInterfaceTransform;
    private Rigidbody deviceInterfaceRb;

    private float currentSample;

    // Start is called before the first frame update
    void Start()
    {
        displayBuffer = new float[sampleRate / 10];
        wavePixels = new Color32[sampleRate / 10];

        waveTexture = new Texture2D(sampleRate / 10, 1, TextureFormat.RGBA32, false);
        waveTexture.filterMode = FilterMode.Point;

        display.texture = waveTexture;
        waveRenderer = display.gameObject.GetComponent<Renderer>();

        deviceInterfaceTransform = transform.Find("deviceInterface");
        deviceInterfaceRb = deviceInterfaceTransform.GetComponent<Rigidbody>();

        sampleBuffer = new float[maxDuration];
        virtualBufferLength = maxDuration;
        sampleRateOverride = AudioSettings.outputSampleRate;

        beatMultiplier = 1 / (Math.Pow(2, 1 / 12.0) * 4);
        lastBeatPeriod = sampleRateOverride / 4;
        biggestBeats = 1;
        biggestPeriod = 1;

        Flush();
        deviceInterface.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentSample = Mathf.Clamp(deviceInterfaceTransform.localPosition.z * sampleRateOverride, 0, sampleRate - 1);

        waveTexture.SetPixels32(wavePixels);
        waveTexture.Apply(false);

        if (displayBufferIndex < displayBuffer.Length)
        {
            displayBuffer[displayBufferIndex] = (float)currentSample / (float)sampleRate;
            displayBufferIndex++;
        }

        if (deviceInterface.recordCountdown || deviceInterface.playCountdown)
        {
            scrubTransform.gameObject.SetActive(true);
            if (!playing)
            {
                return;
            }

            int pixelsPerBar = (int)Mathf.Lerp(0, sampleRate / 10, deviceInterface.countdownTimer / deviceInterface.countdownDuration);
            for (int i = 0; i < pixelsPerBar; i++)
            {
                Color color = Color.Lerp(Color.black, Color.white, (float)i / pixelsPerBar);
                wavePixels[i] = color;
                wavePixels[waveTexture.width - 1 - i] = color;
            }
        }
        else
        {
            scrubTransform.gameObject.SetActive(false);
        }

        scrubTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360, currentSample));
    }

    public void UpdateDuration(float beats, double beatperiod)
    {
        virtualBufferLength = Mathf.RoundToInt(beats * beatperiod * sampleRateOverride *.25f);
        if (beatperiod > biggestPeriod || beats > biggestBeats)
        {
            if (biggestBeats < beats) biggestBeats = beats;
            if (biggestPeriod < beatperiod) biggestPeriod = beatperiod;
            Array.Resize<float>(ref sampleBuffer, Mathf.RoundToInt(biggestBeats * biggestPeriod * sampleRateOverride *.25f));
        }

        beatMultiplier = 1 / (Math.Pow(2, 1 / 12.0) * 4);
        lastBeatPeriod = sampleRateOverride / 4;
        recalcTex();
    }

    private int centerBar = (int)Mathf.Floor(waveTexture.width *.5f);

    private void recalcTex()
    {
        int centerH = waveTexture.height / 2;

        for (int i = 0; i < waveTexture.width; i++)
        {
            if (virtualBufferLength > i)
            {
                int curH = Mathf.FloorToInt((waveTexture.height - 1) *.5f * Mathf.Clamp01(Mathf.Abs(sampleBuffer[i])));

                for (int i2 = 0; i2 < centerH; i2++)
                {
                    if (i2 < curH) wavePixels[(centerH - i2) * waveTexture.width + i] = wavePixels[(centerH + i2) * waveTexture.width + i] = Color.white;
                    else wavePixels[(centerH - i2) * waveTexture.width + i] = wavePixels[(centerH + i2) * waveTexture.width + i] = Color.black

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