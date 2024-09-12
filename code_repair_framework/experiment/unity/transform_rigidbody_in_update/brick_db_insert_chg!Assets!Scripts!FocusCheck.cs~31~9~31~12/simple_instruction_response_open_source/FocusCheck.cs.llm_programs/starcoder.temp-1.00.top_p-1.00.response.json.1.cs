using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;



    
    void Start()
    {
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;

    void Start()
    {
        rb10 = GetComponent<Rigidbody>();
        text.text = "Time Remaining: " + timeLimit.ToString("F2");
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (timeLimit > 0f)
        {
            timeLimit -= Time.deltaTime;
            text.text = "Time Remaining: " + timeLimit.ToString("F2");
            yield return null;
        }
        text.text = "YOU LOSE!";
        Debug.Log("YOU LOSE!");
        // Restart
        Time.timeScale = 1
}
