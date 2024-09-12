
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