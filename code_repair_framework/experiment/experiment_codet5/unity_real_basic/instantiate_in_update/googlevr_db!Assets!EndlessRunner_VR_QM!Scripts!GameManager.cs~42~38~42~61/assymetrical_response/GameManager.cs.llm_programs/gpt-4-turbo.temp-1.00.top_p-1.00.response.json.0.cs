using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject enemyPrefab;
    public Player player;

    private float enemyPointer;
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
        finishLine = GameObject.Find("FinishLine");
        finishLine.transform.position = new Vector3(0,0,finishLinePosition);

        for (int i = 0; i < enemyPoolSize; i++) {
            GameObject enemyObject = Instantiate(enemyPrefab);
            enemyObject.SetActive(false);
            enemyPool.Enqueue(enemyObject);
        }
    }





    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    public int enemyPoolSize = 10;

    void Update() {
        if (enemyPointer < player.cameraRig.transform.position.z) {
            enemyPointer += spawnDistanceFromEnemy;
            SpawnEnemy();
        }

        gameTimer += Time.deltaTime; 

        if (isGamerOver == false) {
            uiText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "   Score: " + player.score  +  "   Speed: " + Mathf.Floor(player.speed) ;

            if (player.reachedFinishLine == true) { 
                isGamerOver = true;
                finalTime = gameTimer;
                finalScore = player.score;
            }
        } else {
            uiText.text = "Game Over!\nTime: " + Mathf.FloorToInt(finalTime) + "   Score: " + finalScore + "\nRestarting in: " + Mathf.Floor(gameOverTimer);
            player.speed = 0.3f;  
            player.maxSpeed = 0.75f;

            gameOverTimer -= Time.deltaTime;
            if (gameOverTimer <= 0) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            }
        }
    }

    void SpawnEnemy() {
        if (enemyPool.Count > 0) {
            GameObject enemyObject = enemyPool.Dequeue();
            enemyObject.transform.position = new Vector3(Random.Range(-4f, 4f), Random.Range(2.5f, 6.5f), player.cameraRig.transform.position.z + spawnDistanceFromPlayer);
            enemyObject.SetActive(true);
        }
    }




}
