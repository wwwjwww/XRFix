using UnityEngine;

public class PanelStats : MonoBehaviour
{
    public Text playerName;
    public Text playerLevel;
    public Text playerHealth;
    public Text playerMana;

    void Start()
    {
        playerName.text = "Player Name: " + PlayerPrefs.GetString("playerName");
        playerLevel.text = "Level: " + PlayerPrefs.GetInt("playerLevel");
        playerHealth.text = "Health: " + PlayerPrefs.GetInt("playerHealth");
        playerMana.text = "Mana: " + PlayerPrefs.GetInt("playerMana");
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
