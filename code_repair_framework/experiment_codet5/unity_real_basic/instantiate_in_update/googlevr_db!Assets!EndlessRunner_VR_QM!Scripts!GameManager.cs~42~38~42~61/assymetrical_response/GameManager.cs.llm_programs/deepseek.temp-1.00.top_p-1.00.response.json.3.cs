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

     


    void Start() {
        finishLine = GameObject.Find("FinishLine");   // Import the Finish Line game object 
        finishLine.transform.position = new Vector3(0,0,finishLinePosition);  // Location to place the finish line object
    }





void Update() {
    if (!isGameOver)
        gameTimer += Time.deltaTime;
}

public int score;

public void IncreaseScore(int amount) {
    score += amount;
}

void Update() {
    enemyPointer += Time.deltaTime;
    if (enemyPointer >= 3f) {
        SpawnEnemy();
        enemyPointer = 0f;
    }
}

void SpawnEnemy() {
    Instantiate(enemyPrefab, transform.position + new Vector3(Random.Range(-spawnDistanceFromPlayer, spawnDistanceFromPlayer), Random.Range(-spawnDistanceFromEnemy, spawnDistanceFromEnemy), 0f), Quaternion.identity);
}




}
