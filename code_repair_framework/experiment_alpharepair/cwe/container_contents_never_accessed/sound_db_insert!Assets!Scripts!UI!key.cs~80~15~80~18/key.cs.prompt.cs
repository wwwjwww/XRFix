













using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class key : manipObject {

  public int keyValue = 0;
  public Material onMat;
  Renderer rend;
  Material offMat;
  Material glowMat;
  deviceInterface _deviceInterface;

  public bool sticky = true;

  Color glowColor = Color.HSVToRGB(.4f, .5f, .1f);

  public bool isKeyboard = false;

  public override void Awake() {
    base.Awake();
    _deviceInterface = transform.parent.GetComponent<deviceInterface>();
    rend = GetComponent<Renderer>();
    offMat = rend.material;
    glowMat = new Material(onMat);
    glowMat.SetColor("_TintColor", glowColor);
  }

  bool initialized = false;
  void Start() {
    initialized = true;
  }

  public void setOffMat(Material m) {
    rend.material = m;
    offMat = rend.material;
  }

  public bool isHit = false;

  public void keyHitCheck() {
    if (!initialized) return;
    bool on = touching || curState == manipState.grabbed || toggled;

    if (on != isHit) {
      isHit = on;
      _deviceInterface.hit(on, keyValue);
    }
  }

  enum keyState {
    off,
    touched,
    grabbedOn,
    grabbedOff,
    selectedOff,
    selectedOn
  };

  int desireSetSelect = 0;
  public void setSelectAsynch(bool on) {
    desireSetSelect = on ? 1 : 2;
  }

  bool phantomHitUpdate = false;
  // BUG: Container contents are never accessed
  // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
  //   Queue<bool> hits = new Queue<bool>();

  //Remove or Commented-out the collection if it is no longer needed
  // FIXED CODE:
