// Assets/Scripts/PanelStats.cs
using UnityEngine;

public class PanelStats : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI text;

    // FIXED CODE:
    private void Update()
    {
        text.text = PanelStats.Format(panel.gameObject.name, panel.gameObject.transform.position);
    }

    // FIXED CODE:
    public static string Format(string panelName, Vector3 position)
    {
        return $"{panelName} at ({position.x:F2}, {position.y:F2}, {position.z:F2})";
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
