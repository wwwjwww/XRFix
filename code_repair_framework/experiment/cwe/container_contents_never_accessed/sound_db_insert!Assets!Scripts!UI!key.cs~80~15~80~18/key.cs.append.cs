
  bool phantomHitUpdate = false;
  Queue<bool> hits = new Queue<bool>();
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