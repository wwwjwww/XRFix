using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace NanIndustryVR
{
    public class PanelStats : MonoBehaviour
    {
        public Transform marker;

        protected Rigidbody rb3;

        protected GameObject gobj1;
        protected GameObject a1;
        private Queue<GameObject> obj_pool = new Queue<GameObject>();
        private float timeLimit = 5f;
        private float timer = 0f;
        private bool instantiate_gobj = false;

        public void UpdateStats(Level.LevelFile level, int index, int local_result)
        {
            string stats = level.score_stats;
            int index_base = index * 24;
            if (stats == null || stats.Length < index_base + 24)
            {
                transform.Find("Number Right").GetComponent<Text>().text = "(no data)";
                transform.Find("Stats").gameObject.SetActive(false);
                marker.gameObject.SetActive(false);
                return;
            }
            int.TryParse(stats.Substring(index_base, 

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
