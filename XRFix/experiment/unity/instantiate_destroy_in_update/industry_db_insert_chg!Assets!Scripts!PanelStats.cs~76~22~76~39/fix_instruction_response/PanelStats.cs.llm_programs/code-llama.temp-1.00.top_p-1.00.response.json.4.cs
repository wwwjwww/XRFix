using UnityEngine;
using System.Collections.Generic;

public class PanelStats : MonoBehaviour
{
    public GameObject statsContainer;
    public GameObject statLabel;
    public TextMeshProUGUI averageScoreText;

    void Start()
    {
        List<int> scores = new List<int>();
        scores.Add(100);
        scores.Add(200);
        scores.Add(300);

        int totalScore = 0;
        for (int i = 0; i < scores.Count; i++)
        {
            totalScore += scores[i];
        }
        averageScore = totalScore / scores.Count;
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
