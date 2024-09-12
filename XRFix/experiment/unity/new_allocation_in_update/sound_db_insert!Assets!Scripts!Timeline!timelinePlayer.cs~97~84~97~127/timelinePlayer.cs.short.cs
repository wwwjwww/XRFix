using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  void Update() {
          // BUG: Using New() allocation in Update() method.
          // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
          //           activeEvents[entry.Key] = _deviceInterface.SpawnTimelineEvent(entry.Key, new Vector2(entry.Value, entry.Value + .01f));

          // FIXED CODE:
