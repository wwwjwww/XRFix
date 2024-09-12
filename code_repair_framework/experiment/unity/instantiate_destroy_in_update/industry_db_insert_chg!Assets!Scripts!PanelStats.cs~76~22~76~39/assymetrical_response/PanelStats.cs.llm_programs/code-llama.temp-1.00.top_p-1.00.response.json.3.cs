//Fixed code:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelStats : MonoBehaviour
{
    public Transform marker;

    protected Rigidbody rb3;

    protected GameObject gobj1;
    protected GameObject a1;

    private float timeLimit = 5f;
    private float timer  = 0f; 
    private bool instantiate_gobj = false;

    void Start()
    {
        a1 = Instantiate(gobj1);
        instantiate_gobj = true;
        rb3 = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer+=Time.deltaTime;
    
        if (instantiate_gobj && timer >= timeLimit){
            Destroy(a1);
            timer = 0;
            instantiate_gobj = false;
        }
    
        rb3.transform.Translate(0, 0, Time.deltaTime * 2);
    
        var p = marker.localPosition;
        p.y = 30f + 4f * Mathf.Sin(Time.time * 5f);
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
