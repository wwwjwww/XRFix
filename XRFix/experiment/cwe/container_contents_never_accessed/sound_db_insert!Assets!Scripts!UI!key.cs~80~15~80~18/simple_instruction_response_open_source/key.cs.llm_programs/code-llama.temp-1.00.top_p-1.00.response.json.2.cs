













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
  
  
void Start() {
    initialized = true;
    _deviceInterface = transform.parent.GetComponent<deviceInterface>();
    rend = GetComponent<Renderer>();
    offMat = rend.material;
    glowMat = new Material(onMat);
    glowMat.SetColor("_TintColor", glowColor);

    // Initialize the desired select state
    desireSetSelect = 0;

    // Initialize the phantom hit update flag
    phantomHitUpdate = false;
  }

  // Update is called once per frame
  void Update() {
    if (!initialized) return;

    // Check if the key is hit
    bool on = touching || curState == manipState.grabbed || toggled;

    // Update the isHit flag
    if (on != isHit) {
      isHit = on;
      _deviceInterface.hit(on, keyValue);
    }

    // Update the desired select state
    if (desireSetSelect != 0) {
      bool on = desireSetSelect == 1;
      _deviceInterface.select(on, keyValue);
      desireSetSelect = 0;
    }

    // Check if the phantom hit update flag is set
    if (phantomHitUpdate) {
      _deviceInterface.phantomHit(true, keyValue);
      phantomHitUpdate = false;
    }
  }

  // Method to set the off material
  public void setOffMat(Material m) {
    rend.material = m;
    offMat = rend.material;
  }

  // Method to set the sticky flag
  public void setSticky(bool sticky) {
    this.sticky = sticky;
  }

  // Method to set the isHit flag
  public void setHit(bool on) {
    isHit = on;
    _deviceInterface.hit(on, keyValue);
  }

  // Method to set the keyboard flag
  public void setKeyboard(bool isKeyboard) {
    this.isKeyboard = isKeyboard;
  }

  // Method to update the desired select state asynchronously
  public void setSelectAsynch(bool on) {
    desireSetSelect = on ? 1 : 2;
  }

  // Method to update the phantom hit state asynchronously
  public void setPhantomHitAsynch(bool on) {
    phantomHitUpdate = on;
  }

  public void phantomHit(bool on) {
    phantomHitUpdate = true;
    isHit = on;
  }

  bool prevToggle = false;
  void Update() {
    if (phantomHitUpdate) {
      curSelect = 0;
      phantomHitUpdate = false;
      if (isHit) {
        if (isKeyboard) setKeyFeedbackState(keyState.selectedOn);
        else {
          if (toggled) setKeyFeedbackState(keyState.touched);
          else setKeyFeedbackState(keyState.selectedOff);
        }
      } else {
        setKeyFeedbackState(keyState.off);
      }
    }

    if (!sticky || !isHit) return;

    if (desireSetSelect != 0) {
      curSelect = desireSetSelect;
      if (desireSetSelect == 1) {
        if (toggled) setKeyFeedbackState(keyState.grabbedOn);
        else setKeyFeedbackState(keyState.selectedOn);
      } else {
        if (toggled) setKeyFeedbackState(keyState.touched);
        else setKeyFeedbackState(keyState.selectedOff);
      }

      desireSetSelect = 0;
    }

    if (prevToggle != toggled && isHit) {
      prevToggle = toggled;
      if (curSelect == 1) {
        if (toggled) setKeyFeedbackState(keyState.grabbedOn);
        else setKeyFeedbackState(keyState.selectedOn);
      } else {
        if (toggled) setKeyFeedbackState(keyState.touched);
        else setKeyFeedbackState(keyState.selectedOff);
      }
    }

  }

  int curSelect = 0;

  bool touching = false;
  public override void onTouch(bool on, manipulator m) {
    touching = on;
    if (m != null) {
      if (on) m.hapticPulse(3000);
      else m.hapticPulse(700);
    }

    keyHitCheck();
  }


  public bool toggled = false;
  public override void setState(manipState state) {
    if (!sticky) return;

    bool lateHitCheck = false;
    if (curState == manipState.grabbed && state != manipState.grabbed) {
      lateHitCheck = true;
    }

    curState = state;

    if (curState == manipState.grabbed) {
      toggled = !toggled;
      keyHitCheck();
    } else if (lateHitCheck) keyHitCheck();
  }

  void setKeyFeedbackState(keyState s) {
    switch (s) {
      case keyState.off:
        rend.material = offMat;
        break;
      case keyState.touched:
        rend.material = glowMat;
        setKeyColor(.65f, .3f);
        break;
      case keyState.grabbedOn:
        rend.material = glowMat;
        setKeyColor(.4f, .4f);
        break;
      case keyState.grabbedOff:
        rend.material = glowMat;
        setKeyColor(.65f, .4f);
        break;
      case keyState.selectedOff:
        rend.material = glowMat;
        setKeyColor(.4f, .3f);
        break;
      case keyState.selectedOn:
        rend.material = glowMat;
        setKeyColor(.5f, .4f);
        break;
      default:
        break;
    }
  }

  public void setKeyColor(float hue, float gain) {
    rend.material.SetColor("_TintColor", Color.HSVToRGB(hue, .9f, .5f));
    rend.material.SetFloat("_EmissionGain", gain);
  }
}