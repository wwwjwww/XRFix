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

     


void Start()
{
    finishLine = GameObject.Find("FinishLine");   // Import the Finish Line game object 
    finishLine.transform.position = new Vector3(0, 0, finishLinePosition);  // Location to place the finish line object
    CreateEnemyPool();
}





private List<GameObject> enemyPool = new List<GameObject>();

void CreateEnemyPool()
{
    for (int i = 0; i < 10; i++)
    {
        GameObject enemyObject = Instantiate(enemyPrefab);
        enemyObject.SetActive(false);
        enemyPool.Add(enemyObject);
    }
}

void Update()
{
    if (enemyPointer < player.cameraRig.transform.position.z)
    {
        enemyPointer += spawnDistanceFromEnemy;
        SpawnEnemy();
    }

    gameTimer += Time.deltaTime;   // Increment the game timer

    if (!isGamerOver)
    {
        uiText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "   Score: " + player.score + "   Speed: " + Mathf.Floor(player.speed);

        if (player.reachedFinishLine)
        {
            isGamerOver = true;
            finalTime = gameTimer;
            finalScore = player.score;
        }
    }
    else
    {
        uiText.text = "Game Over!\nTime: " + Mathf.FloorToInt(finalTime) + "   Score: " + finalScore + "\nRestarting in: " + Mathf.Floor(gameOverTimer);
        player.speed = 0.3f; player.maxSpeed = 0.75f;

        gameOverTimer -= Time.deltaTime;    // Gamer restart logic
        if (gameOverTimer <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

void SpawnEnemy()
{
    foreach (GameObject enemy in enemyPool)
    {
        if (!enemy.activeInHierarchy)
        {
            enemy.transform.position = new Vector3(Random.Range(-4f, 4f), Random.Range(2.5f, 6.5f), player.cameraRig.transform.position.z + spawnDistanceFromPlayer);
            enemy.SetActive(true);
            break;
        }
    }
}




}
