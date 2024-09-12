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

        private float timeLimit = 5f;
        private float timer  = 0f; 
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
            int.TryParse(stats.Substring(index_base, 4), out int step_size);
            if (step_size <= 0)
                step_size = 1;

            var tr = transform.Find("Number Right");
            tr.GetComponent<Text>().text = (step_size * 10).ToString();

            tr = transform.Find("Stats");
            tr.gameObject.SetActive(true);
            for (int i = 0; i < 10; i++)
            {
                int.TryParse(stats.Substring(index_base + 4 + 2 * i, 2), out int stat);
                var rtr = tr.GetChild(i) as RectTransform;
                rtr.gameObject.SetActive(stat > 0);
                if (stat > 0)
                {
                    stat += 1;   
                    rtr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, stat);
                }
            }

            local_result = (local_result - 1) / step_size;
            if (local_result < 10)
            {
                marker.localRotation = Quaternion.Euler(0, 0, 180);
                marker.localPosition = tr.GetChild(local_result).localPosition;
            }
            else
            {
                marker.localRotation = Quaternion.Euler(0, 0, -90);
                marker.localPosition = new Vector3((tr as RectTransform).rect.width * 0.5f, 30, 0);
            }
            marker.gameObject.SetActive(true);
        }





Unity и Unreal Engine - это два из самых популярных движков игры и визуализации на данный момент. Основное различие между ними - это их фокус. Unity был создан для создания игр и других визуальных приложений, а Unreal Engine - для создания мультимедийных игр и приложений.

Unity использует язык программирования C # и специальный движок для графики в real-time, который называется "Unity Engine". Его можно использовать для создания игр, анимаций, визуальных эффектов и других визуальных приложений. Unity также поддерживает множество платформ, включая Windows, MacOS, Linux, PlayStation, Xbox и другие.

Unreal Engine же использует свой собственный язык программирования, называемый "Unreal Script", который основан на C++. Он также имеет собственный движок графики, называемый "Unreal Engine 4". Unreal Engine также имеет более высокое разрешение, чем Unity, и поддерживает более широкий спектр платформ, включая различные видеоигры Consoles.

В целом, Unity и Unreal Engine имеют некоторые общие возможности, такие как поддержка модели совместимости и создание пользовательских интерфейсов. Однако Unreal Engine имеет более высокое разрешение и более гибкий движок графики.
<|user|>
А Open Assitant может помочь разработчику в Unity и Unreal Engine?
<|system|>

<|user|>
Why do doctors make house calls?

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
