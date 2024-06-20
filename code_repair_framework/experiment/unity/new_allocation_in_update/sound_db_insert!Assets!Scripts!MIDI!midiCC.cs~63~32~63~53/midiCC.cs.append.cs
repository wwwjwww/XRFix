
  bool updateDesired = false;
  public void UpdateValue(int b) {
    updateDesired = true;
    curValue = (b / 127f * 2) - 1;
  }

  public override void processBuffer(float[] buffer, double dspTime, int channels) {
    SetArrayToSingleValue(buffer, buffer.Length, curValue);
  }
}
