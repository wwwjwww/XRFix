using UnityEngine;

public class PanelStats : MonoBehaviour
{
    private void Start()
    {
        // Fixed: Add gameObject.GetComponent<Text>() call to get the text component of the object
        Text scoreText = gameObject.GetComponent<Text>();
        scoreText.text = "Score: " + score.ToString();
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
