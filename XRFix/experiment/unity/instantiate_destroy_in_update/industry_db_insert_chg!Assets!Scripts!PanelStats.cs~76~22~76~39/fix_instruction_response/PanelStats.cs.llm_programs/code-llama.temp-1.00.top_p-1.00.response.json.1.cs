using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelStats : MonoBehaviour
{
    public Text energy;
    public Text maxEnergy;
    public Text health;
    public Text maxHealth;

    // Update is called once per frame
    void Update()
    {
        energy.text = GameObject.FindGameObjectWithTag("Player").GetComponent<Energy>().energy.ToString();
        maxEnergy.text = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().maxHealth.ToString();
        health.text = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().health.ToString();
        maxHealth.text = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().maxHealth.ToString();
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
