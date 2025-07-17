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

     


    void Start()
    {
        finishLine = GameObject.Find("FinishLine");
        finishLine.transform.position = new Vector3(0, 0, finishLinePosition);

        enemyPool = new Queue<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemyObject = Instantiate(enemyPrefab);
            enemyObject.SetActive(false);
            enemyPool.Enqueue(enemyObject);
        }

        StartCoroutine(SpawnEnemies());
    }


///     void Update() {
// 
// 
//         if (enemyPointer < player.cameraRig.transform.position.z) {
//             enemyPointer += spawnDistanceFromEnemy;
// 
// 
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //             GameObject enemyObject = Instantiate(enemyPrefab);
            //             enemyObject.transform.position = new Vector3(Random.Range(-4f, 4f), Random.Range(2.5f, 6.5f), player.cameraRig.transform.position.z + spawnDistanceFromPlayer);
            // 
            //         }
            // 
            //         gameTimer += Time.deltaTime;   // Increment the game timer 
            // 
            // 
            //         if (isGamerOver == false) {
            //             uiText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "   Score: " + player.score  +  "   Speed: " + Mathf.Floor(player.speed) ;
            // 
            //             if (player.reachedFinishLine == true) { // Change Boolean when player hits finish line and save the final time and score
            //                 isGamerOver = true;
            //                 finalTime = gameTimer;
            //                 finalScore = player.score;
            //             }
            //         } else {  // What to do when the player hits the finish line
            //             uiText.text = "Game Over!\nTime: " + Mathf.FloorToInt(finalTime) + "   Score: " + finalScore + "\nRestarting in: " + Mathf.Floor(gameOverTimer);
            //             player.speed = 0.3f;  player.maxSpeed = 0.75f;
            // 
            //             gameOverTimer -= Time.deltaTime;    // Gamer restart logic
            //             if (gameOverTimer <= 0) {
            //                 SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            //             }
            //         }
            // 
            // 
            // 
            //     }

            // FIXED CODE:


    private Queue<GameObject> enemyPool;

    public int initialPoolSize = 5;

    IEnumerator SpawnEnemies()
    {
        while (!isGamerOver)
        {
            if (enemyPointer < player.cameraRig.transform.position.z)
            {
                enemyPointer += spawnDistanceFromEnemy;

                GameObject enemyObject = GetPooledEnemy();
                enemyObject.transform.position = new Vector3(
                    Random.Range(-4f, 4f),
                    Random.Range(2.5f, 6.5f),
                    player.cameraRig.transform.position.z + spawnDistanceFromPlayer);
                enemyObject.SetActive(true);
            }

            yield return new WaitForSeconds(1f); 
        }
    }

    GameObject GetPooledEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }

        GameObject enemyObject = Instantiate(enemyPrefab);
        enemyObject.SetActive(false);
        enemyPool.Enqueue(enemyObject);
        return enemyObject;
    }

    void Update()
    {
        gameTimer += Time.deltaTime;

        if (isGamerOver == false)
        {
            uiText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "   Score: " + player.score + "   Speed: " + Mathf.Floor(player.speed);

            if (player.reachedFinishLine == true)
            {
                isGamerOver = true;
                finalTime = gameTimer;
                finalScore = player.score;
            }
        }
        else
        {
            uiText.text = "Game Over!\nTime: " + Mathf.FloorToInt(finalTime) + "   Score: " + finalScore + "\nRestarting in: " + Mathf.Floor(gameOverTimer);
            player.speed = 0.3f;
            player.maxSpeed = 0.75f;

            gameOverTimer -= Time.deltaTime;
            if (gameOverTimer <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }




}
