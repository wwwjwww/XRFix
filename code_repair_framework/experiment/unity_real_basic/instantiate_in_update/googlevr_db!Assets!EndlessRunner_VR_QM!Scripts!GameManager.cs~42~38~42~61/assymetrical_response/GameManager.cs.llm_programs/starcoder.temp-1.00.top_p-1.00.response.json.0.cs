using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject enemyPrefab;
    public Player player;

    private float enemyPointer;     // Calculator for distance of player to enemy
    public float spawnDistanceFromPlayer = 20f;
    public float spawnDistanceFromEnemy = 5f;

    public TextMesh uiText;

    public float gameTimer;
    private float finalTime;
    private int finalScore;
    private bool isGamerOver = false;
    private float gameOverTimer = 4.5f;

    public GameObject finishLine;
    public float finishLinePosition = 200f;

     


    private void Start()
    {
        finishLine = GameObject.Find("FinishLine");
        finishLine.transform.position = new Vector3(0, 0, finishLinePosition);
    }





    void Update()
    {
        enemyPointer = Vector3.Distance(player.transform.position, enemyPrefab.transform.position);

        if (enemyPointer < spawnDistanceFromPlayer && enemyPrefab.activeSelf == false)
        {
            Instantiate(enemyPrefab, new Vector3(Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f)), enemyPrefab.transform.rotation);
            enemyPrefab.SetActive(true);
        }

        if (enemyPointer > spawnDistanceFromEnemy && enemyPrefab.activeSelf == true)
        {
            enemyPrefab.SetActive(false);
        }

        if (player.transform.position.z >= finishLinePosition && isGamerOver == false)
        {
            isGamerOver = true;
            finalTime = Time.time - gameTimer;
            finalScore = PlayerPrefs.GetInt("HighScore", 0);

            if (finalTime < PlayerPrefs.GetFloat("BestTime", Mathf.Infinity))
            {
                PlayerPrefs.SetFloat("BestTime", finalTime);
                PlayerPrefs.SetInt("HighScore", finalScore);
            }

            uiText.text = $"Game Over!\nYour Score: {finalScore}\nBest Time: {PlayerPrefs.GetFloat("BestTime", Mathf.Infinity):F2}";
            Invoke("LoadMenu", gameOverTimer);
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }




}
