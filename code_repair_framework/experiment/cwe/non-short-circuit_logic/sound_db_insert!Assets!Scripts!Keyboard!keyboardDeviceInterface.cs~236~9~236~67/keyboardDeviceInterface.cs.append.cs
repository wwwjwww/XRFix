
  public enum keyInput {
    seq,
    midi,
    touch
  }

  struct keyState {
    public bool seqState;
    public bool midiState;
    public bool touchState;

    public bool currentState;
    public bool currentNonSeqState;

    public keyState(bool on) {
      currentNonSeqState = currentState = seqState = midiState = touchState = on;
    }

    public bool getState() {
      return seqState || midiState || touchState;
    }

    public bool getNonSeqState() {
      return midiState || touchState;
    }

    public bool stateChange() {
      return getState() != currentState;
    }

    public bool nonSeqStateChange() {
      return getNonSeqState() != currentNonSeqState;
    }
  };
}


public class KeyboardData : InstrumentData {
  public Vector2[] ADSRdata;
  public bool muted;
  public bool octaveSwitch;
  public bool sequencerEnabled;
  public int jackOutID;
  public int freqOutID;
  public int ampOutID;
  public string midiInConnection;
  public string midiOutConnection;

  public bool timelinePresent;
  public TimelineComponentData timelineData;
  public timelineEvent.eventData[] timelineEvents;
  public float timelineHeight;
}