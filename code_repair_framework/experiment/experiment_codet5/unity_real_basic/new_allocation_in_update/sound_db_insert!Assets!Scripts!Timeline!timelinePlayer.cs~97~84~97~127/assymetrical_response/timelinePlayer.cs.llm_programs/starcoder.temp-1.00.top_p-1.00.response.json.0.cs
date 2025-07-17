













using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timelinePlayer : MonoBehaviour {
  public xHandle playheadHandle;
  timelineComponentInterface _deviceInterface;
  float lastBeatTime = 0;

  float curGridPosition;

  public bool looping = true;

  Dictionary<int, timelineEvent> activeEvents = new Dictionary<int, timelineEvent>();

  void Awake() {
    _deviceInterface = GetComponent<timelineComponentInterface>();
  }

  void Start() {
    masterControl.instance.beatUpdateEvent += beatUpdateEvent;
    masterControl.instance.beatResetEvent += beatResetEvent;
  }

  void OnDestroy() {
    masterControl.instance.beatUpdateEvent -= beatUpdateEvent;
    masterControl.instance.beatResetEvent -= beatResetEvent;
  }

  void beatResetEvent() {
    lastBeatTime = 0;
    Back();
  }

  public void setRecord(bool on) {
    List<int> keys = new List<int>(activeEvents.Keys);
    foreach (int n in keys) {
      activeEvents[n].setRecord(false);
      activeEvents.Remove(n);
    }
  }

  void updatePlayhead() {
    if (_deviceInterface._gridParams.isOnGrid(curGridPosition)) {
      playheadHandle.gameObject.SetActive(true);
      Vector3 pos = playheadHandle.transform.localPosition;
      pos.x = _deviceInterface._gridParams.UnittoX(curGridPosition);
      playheadHandle.transform.localPosition = pos;
    } else playheadHandle.gameObject.SetActive(false);
  }

  List<int> loopKeys = new List<int>();
  void loopActiveEvents() {
    loopKeys = new List<int>(activeEvents.Keys);
    foreach (int n in loopKeys) {
      activeEvents[n].setOut(_deviceInterface._gridParams.head_tail.y);
      activeEvents[n].setRecord(false);
      activeEvents.Remove(n);
    }
  }

  public void Back() {
    curGridPosition = _deviceInterface._gridParams.head_tail.x;
  }

  bool playheadScrubbing = false;

  void Update() {
    playheadScrubbing = playheadHandle.curState == manipObject.manipState.grabbed;

    if (!playheadScrubbing) updatePlayhead();
    else {
      playheadHandle.xBounds.y = _deviceInterface._gridParams.UnittoX(_deviceInterface._gridParams.head_tail.x);
      playheadHandle.xBounds.x = _deviceInterface._gridParams.UnittoX(_deviceInterface._gridParams.head_tail.y);
      curGridPosition = _deviceInterface._gridParams.XtoUnit(playheadHandle.transform.localPosition.x);
      timelineEventUpdate();
    }

    lock (_recordLock) {

      if (toRecord.Keys.Count > 0) {
        foreach (KeyValuePair<int, float> entry in toRecord) {
          /* BUG: Using New() allocation in Update() method.
          * MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
          *           activeEvents[entry.Key] = _deviceInterface.SpawnTimelineEvent(entry.Key, new Vector2(entry.Value, entry.Value + .01f));

          * Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
          * FIXED CODE:
          */
          using System.Collections.Generic;

public class timelinePlayer : MonoBehaviour
{
    public xHandle playheadHandle;
    Dictionary<int, timelineEvent> activeEvents = new Dictionary<int, timelineEvent>();
    Dictionary<int, float> toRecord = new Dictionary<int, float>();
    object _recordLock = new object();

    float curGridPosition;

    public bool looping = true;

    void Start()
    {
        masterControl.instance.beatUpdateEvent += beatUpdateEvent;
    }

    void OnDestroy()
    {
        masterControl.instance.beatUpdateEvent -= beatUpdateEvent;
    }

    public void record(int id, float time)
    {
        lock (_recordLock)
        {
            toRecord[id] = time;
        }
    }

    void beatUpdateEvent()
    {
        lock (_recordLock)
        {
            foreach (KeyValuePair<int, float> entry in toRecord)
            {
                activeEvents[entry.Key] = new timelineEvent(entry.Key, entry.Value, false, false, false);
            }
            toRecord.Clear();
        }
        timelineEventUpdate();
    }

    void timelineEventUpdate()
    {
        List<int> keys = new List<int>(activeEvents.Keys);
        foreach (int n in keys)
        {
            activeEvents[n].update(_deviceInterface._gridParams.head_tail.y);
        }
    }

    public void Back()
    {
        curGridPosition = _deviceInterface._gridParams.head_tail.x;
    }

    bool playheadScrubbing = false;

    void Update()
    {
        playheadScrubbing = playheadHandle.curState == manipObject.manipState.grabbed;

        if (!playheadScrubbing) updatePlayhead();
        else
        {
            playheadHandle.xBounds.y = _deviceInterface._gridParams.UnittoX(_deviceInterface._gridParams.head_tail.x);
            playheadHandle.xBounds.x = _deviceInterface._gridParams.UnittoX(_deviceInterface._gridParams.head_tail.y);
            curGridPosition = _deviceInterface._gridParams.XtoUnit(playheadHandle.transform.localPosition.x);
            timelineEventUpdate();
        }
    }

    class timelineEvent
    {
        public int id { get; set; }
        public float time { get; set; }
        public float outValue { get; set; }
        public bool looping { get; set; }
        public bool record { get; set; }
        public bool recordActive { get; set; }

        public timelineEvent(int id, float time, bool looping, bool record, bool recordActive)
        {
            this.id = id;
            this.time = time;
            this.looping = looping;
            this.record = record;
            this.recordActive = recordActive;
            outValue = 0;
        }

        public void setOut(float val)
        {
            outValue = val;
        }

        public void update(float gridLength)
        {
            if (recordActive)
            {
                recordActive = false;
                record = false;
            }

            if (record)
            {
                record = false;
                recordActive = true;
            }

            if (_deviceInterface._gridParams.UnittoX(curGridPosition) >= time &&!_deviceInterface._gridParams.isOnGrid(curGridPosition + gridLength))
            {
                looping = false;
                record = false;
                recordActive = false;
            }
        }

        public void setRecord(bool val)
        {
            record = val;
        }
    }
}

          activeEvents[entry.Key].setRecord(true);
          activeEvents[entry.Key].setOut(entry.Value + .01f);
          activeEvents[entry.Key].overlapCheck();
        }

        toRecord.Clear();
      }
    }
  }

  public void clearActiveEvents() {
    activeEvents.Clear();
  }

  private object _recordLock = new object();
  Dictionary<int, float> toRecord = new Dictionary<int, float>();

  public void RecordEvent(int n, bool on) {
    if (on) {
      if (activeEvents.ContainsKey(n)) {
        activeEvents[n].setRecord(false);
        activeEvents.Remove(n);
      }

      float b = curGridPosition;
      if (_deviceInterface.snapping) {
        b = _deviceInterface._gridParams.UnittoSnap(b, true);
      }
      lock (_recordLock) toRecord[n] = b;
    } else {
      if (activeEvents.ContainsKey(n)) {
        activeEvents[n].setRecord(false);
        activeEvents.Remove(n);
      } else {
        lock (_recordLock) {
          if (toRecord.ContainsKey(n)) toRecord.Remove(n);
        }

      }
    }
  }

  void timelineEventUpdate() {
    for (int i = _deviceInterface._tlEvents.Count - 1; i >= 0; i--) {
      if (i < _deviceInterface._tlEvents.Count) {
        if (_deviceInterface._tlEvents[i] != null) {
          if (_deviceInterface._tlEvents[i].recording) {
            _deviceInterface._tlEvents[i].setOut(curGridPosition);
            _deviceInterface._tlEvents[i].overlapCheck();
          } else if (_deviceInterface._tlEvents[i].grabbed) {
            // nothing?
          } else if (!_deviceInterface._tlEvents[i].inRange(curGridPosition)) {
            if (_deviceInterface._tlEvents[i].playing) _deviceInterface.tlEventTrigger(i, false);
          } else {
            bool shouldplay = true;

            if (_deviceInterface.recording) {
              if (!_deviceInterface.overdub) // not overdubbing means overwrite
              {
                if (!_deviceInterface._tlEvents[i].playing) {
                  _deviceInterface._tlEvents[i].setIn(curGridPosition, false);
                  shouldplay = false;
                }
              }
            }

            if (i < _deviceInterface._tlEvents.Count) {
              if (_deviceInterface._tlEvents[i] != null) {
                if (!_deviceInterface._tlEvents[i].playing && shouldplay) {
                  _deviceInterface.tlEventTrigger(i, true);
                }
              }
            }
          }
        }
      }

    }
  }

  bool playing = false;
  bool playDesired = false;
  int playBeat = 0;

  public void setPlay(bool on, bool immediate) {
    playDesired = on;
    if (immediate) playing = playDesired;
    playBeat = Mathf.CeilToInt(curBeatTime * 8) % 8;
  }

  float curBeatTime = 0;
  public bool timeSynch = true;
  public void beatUpdateEvent(float t) {
    if (playDesired != playing) {
      if (playDesired) {
        if (playBeat == Mathf.FloorToInt(curBeatTime * 8) % 8) {
          playing = playDesired;
        }
      } else playing = playDesired;
    }

    curBeatTime = t;

    if (!playing || playheadScrubbing) {
      lastBeatTime = t;
      return;
    }

    if (lastBeatTime > curBeatTime) {
      curGridPosition = curGridPosition + (1 - lastBeatTime + curBeatTime) / _deviceInterface._gridParams.unitDuration;
    } else curGridPosition = curGridPosition + (curBeatTime - lastBeatTime) / _deviceInterface._gridParams.unitDuration;

    bool shouldUpdate = true;
    if (curGridPosition > _deviceInterface._gridParams.head_tail.y) {
      if (looping) {
        float dif = curGridPosition - _deviceInterface._gridParams.head_tail.y;
        curGridPosition = _deviceInterface._gridParams.head_tail.x + dif;
        loopActiveEvents();
      } else {
        curGridPosition = _deviceInterface._gridParams.head_tail.x;
        _deviceInterface.setPlay(false);
        shouldUpdate = false;
      }
    }

    if (curGridPosition < _deviceInterface._gridParams.head_tail.x) curGridPosition = _deviceInterface._gridParams.head_tail.x;
    if (shouldUpdate) timelineEventUpdate();
    lastBeatTime = curBeatTime;
  }
}
