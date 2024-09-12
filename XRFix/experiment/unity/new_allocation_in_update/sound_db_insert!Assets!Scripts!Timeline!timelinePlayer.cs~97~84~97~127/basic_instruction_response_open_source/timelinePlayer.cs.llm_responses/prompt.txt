













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
          // BUG: Using New() allocation in Update() method.
          // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
          //           activeEvents[entry.Key] = _deviceInterface.SpawnTimelineEvent(entry.Key, new Vector2(entry.Value, entry.Value + .01f));

          // FIXED CODE:
