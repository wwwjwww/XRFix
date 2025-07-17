using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///   void Update() {
//     playheadScrubbing = playheadHandle.curState == manipObject.manipState.grabbed;
// 
//     if (!playheadScrubbing) updatePlayhead();
//     else {
//       playheadHandle.xBounds.y = _deviceInterface._gridParams.UnittoX(_deviceInterface._gridParams.head_tail.x);
//       playheadHandle.xBounds.x = _deviceInterface._gridParams.UnittoX(_deviceInterface._gridParams.head_tail.y);
//       curGridPosition = _deviceInterface._gridParams.XtoUnit(playheadHandle.transform.localPosition.x);
//       timelineEventUpdate();
//     }
// 
//     lock (_recordLock) {
// 
//       if (toRecord.Keys.Count > 0) {
//         foreach (KeyValuePair<int, float> entry in toRecord) {
          // BUG: Using New() allocation in Update() method.
          // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
          //           activeEvents[entry.Key] = _deviceInterface.SpawnTimelineEvent(entry.Key, new Vector2(entry.Value, entry.Value + .01f));
          //           activeEvents[entry.Key].setRecord(true);
          //           activeEvents[entry.Key].setOut(entry.Value + .01f);
          //           activeEvents[entry.Key].overlapCheck();
          //         }
          // 
          //         toRecord.Clear();
          //       }
          //     }
          //   }

          // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
          // FIXED CODE:
