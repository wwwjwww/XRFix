
  public override void processBuffer(float[] buffer, double dspTime, int channels) {
    if (incoming == null) return;
    incoming.processBuffer(buffer, dspTime, channels);
    if (curValue != buffer[buffer.Length - 1]) {
      curValue = buffer[buffer.Length - 1];
      updateDesired = true;
      ccMessageDesired = true;
    }
  }
}
