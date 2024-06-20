
  public override InstrumentData GetData() {
    MixerData data = new MixerData();
    data.deviceType = menuItem.deviceType.Mixer;
    data.jackOutID = output.GetInstanceID();

    GetTransformData(data);

    data.jackInID = new int[count * 2];
    data.sliders = new float[count];

    for (int i = 0; i < count; i++) {
      data.sliders[i] = signal.incoming[i].GetComponent<fader>().fadeSlider.percent;
      data.jackInID[2 * i] = signal.incoming[i].GetComponent<fader>().inputA.transform.GetInstanceID();
      data.jackInID[2 * i + 1] = signal.incoming[i].GetComponent<fader>().inputB.transform.GetInstanceID();
    }

    data.height = lengthSlider.localPosition.x;

    return data;
  }

  public override void Load(InstrumentData d) {
    MixerData data = d as MixerData;
    base.Load(data);
    output.GetComponent<omniJack>().ID = data.jackOutID;

    count = data.sliders.Length;
    Vector3 pos = stretchSlider.localPosition;
    pos.x = (count - 1) * -.04f - .076f;
    stretchSlider.localPosition = pos;
    updateMixerCount();

    pos = lengthSlider.localPosition;
    pos.x = data.height;
    lengthSlider.localPosition = pos;

    for (int i = 0; i < count; i++) {
      signal.incoming[i].GetComponent<fader>().fadeSlider.setPercent(data.sliders[i]);
      signal.incoming[i].GetComponent<fader>().inputA.ID = data.jackInID[2 * i];
      signal.incoming[i].GetComponent<fader>().inputB.ID = data.jackInID[2 * i + 1];
    }
  }
}

public class MixerData : InstrumentData {
  public int[] jackInID;
  public float[] sliders;
  public int jackOutID;
  public float height;
}
