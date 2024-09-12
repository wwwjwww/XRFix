using UnityEngine;

namespace NanIndustryVR
{
    public class PanelStats : MonoBehaviour
    {
        public Transform marker;

        protected Rigidbody rb3;

        protected GameObject gobj1;
        private GameObject a1Instance;

        private float timeLimit = 5f;
        private float timer = 0f;
        private bool instantiate_gobj = false;

        private void Start()
        {
            if (!gobj1) return;
            a1Instance = Instantiate(gobj1);
            a1Instance.SetActive(false);
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                if (a1Instance == null)
                {
                    a1Instance = Instantiate(gobj1);
                }
                a1Instance.SetActive(true);
                resetTimer();
            }
            if (instantiate_gobj && timer >= timeLimit)
            {
                a1Instance.SetActive(false);
                resetTimer();
            }

            if (rb3 != null)
            {
                rb3.transform.Translate(0, 0, Time.deltaTime * 2);
            }

            var p = marker.localPosition;
            p.y = 30f + 4f * Mathf.Sin(Time.time * 5f);
            marker.localPosition = p;
        }

        private void resetTimer()
        {
            timer = 0f;
            instantiate_gobj = !instantiate_gobj;
        }
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
