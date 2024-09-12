using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NanIndustryVR
{
    public class PanelStats : MonoBehaviour
    {
        // ... (Other existing code) ...

        // The Update() method
        private void Update()
        {
            // Your existing Update() logic here...
            // (No changes to your existing Update logic)
        }

        // The FixedUpdate() method
        private void FixedUpdate()
        {
            // Previous Rigidbody transformation logic placed in FixedUpdate().
            if (rb3 != null)
            {
                rb3.MovePosition(rb3.position + Vector3.forward * Time.fixedDeltaTime * 2);
            }

            if (marker != null)
            {
                var p = marker.localPosition;
                p.y = 30f + 4f * Mathf.Sin(Time.fixedTime * 5f);
                marker.localPosition = p;
            }
        }

        public static int GetLevelStatsCount(Level.LevelFile level)
        {
            string stats = level.score_stats;
            if (stats == null || stats.Length < 48 + 6)
                return 0;
            int i = 48;
            while (i < 48 + 6 && stats[i] == '=')
                i++;
            int.TryParse(stats.Substring(i, 48 + 6 - i), out int result);
            return result;
        }
    }
}
