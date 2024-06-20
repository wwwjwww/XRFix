// Copyright 2017 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class mixerDeviceInterface : deviceInterface {

  mixer signal;
  public GameObject mixerPrefab;
  public Transform stretchSlider, speaker, output, lengthSlider;

  public List<fader> faderList = new List<fader>();

  int count = 0;

  float faderLength = 0;
  float prevFaderLength = 0;

  public override void Awake() {
    base.Awake();
    signal = GetComponent<mixer>();

    float xVal = stretchSlider.localPosition.x;
    count = Mathf.FloorToInt((xVal + .075f) / -.04f) + 1;
    updateMixerCount();
  }

  void updateMixerCount() {
    int cur = signal.incoming.Count;
    if (count > cur) {
      for (int i = 0; i < count - cur; i++) {
        signalGenerator s = (Instantiate(mixerPrefab, transform, false) as GameObject).GetComponent<signalGenerator>();
        faderList.Add(s as fader);
        s.transform.localPosition = new Vector3(-.03f - .04f * signal.incoming.Count, 0, 0);
        signal.incoming.Add(s);

        float fL = 1 + faderLength * 4f;
        faderList.Last().updateFaderLength(fL);
        Vector3 pos = faderList.Last().transform.localPosition;
        pos.z = -.12f * fL + .12f;
        faderList.Last().transform.localPosition = pos;
      }
    } else // count < cur
       {
      for (int i = 0; i < cur - count; i++) {
        signalGenerator s = signal.incoming.Last();
        faderList.RemoveAt(signal.incoming.Count - 1);
        signal.incoming.RemoveAt(signal.incoming.Count - 1);
        Destroy(s.gameObject);
      }
    }
  }

  void Update() {
    // BUG: Using New() allocation in Update() method.
    // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
    //     float xVal = stretchSlider.localPosition.x;
    //     speaker.localPosition = new Vector3(xVal - .0125f, 0, .11f);
    //     output.localPosition = new Vector3(xVal - .0125f, 0, .14f);
    // 
    //     count = Mathf.FloorToInt((xVal + .075f) / -.04f) + 1;
    //     if (count != signal.incoming.Count) updateMixerCount();
    // 
    // 
    //     faderLength = lengthSlider.localPosition.x;
    //     if (faderLength != prevFaderLength) {
    //       prevFaderLength = faderLength;
    //       float fL = 1 + faderLength * 4f;
    //       for (int i = 0; i < faderList.Count; i++) {
    //         faderList[i].updateFaderLength(fL);
    //         Vector3 pos = faderList[i].transform.localPosition;
    //         pos.z = -.12f * fL + .12f;
    //         faderList[i].transform.localPosition = pos;
    //       }
    //     }
    //   }

    // FIXED VERSION:
