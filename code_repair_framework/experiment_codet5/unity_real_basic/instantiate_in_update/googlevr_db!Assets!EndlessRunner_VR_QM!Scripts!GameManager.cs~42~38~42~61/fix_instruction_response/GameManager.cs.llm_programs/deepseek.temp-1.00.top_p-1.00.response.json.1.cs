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
        
        // Initialize the enemy pool with the enemyPrefab and a pool size of 100
        enemyPool = new ObjectPool(enemyPrefab, 100);
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


    private bool isGameOver = false;

    private ObjectPool enemyPool; // Create an object pool

    void Update() {
        if (enemyPointer < player.cameraRig.transform.position.z) {
            enemyPointer += spawnDistanceFromEnemy;

            // Get a game object from the enemy pool
            GameObject enemyObject = enemyPool.GetObject();
            enemyObject.transform.position = new Vector3(Random.Range(-4f, 4f), Random.Range(2.5f, 6.5f), player.cameraRig.transform.position.z + spawnDistanceFromPlayer);
            enemyObject.SetActive(true); // Make sure to activate the game object from the pool
        }

        gameTimer += Time.deltaTime;   

        if (!isGameOver) {
            uiText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "   Score: " + player.score  +  "   Speed: " + Mathf.Floor(player.speed) ;
            
            if (player.reachedFinishLine) { 
                isGameOver = true;
                finalTime = gameTimer;
                finalScore = player.score;
            }
        } else {  
            uiText.text = "Game Over!\nTime: " + Mathf.FloorToInt(finalTime) + "   Score: " + finalScore + "\nRestarting in: " + Mathf.Floor(gameOverTimer);
            player.speed = 0.3f;  player.maxSpeed = 0.75f;

            gameOverTimer -= Time.deltaTime;    
            if (gameOverTimer <= 0) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            }
        }
    }

public class ObjectPool {
    private List<GameObject> pool; // List to hold the game objects

    public ObjectPool(GameObject obj, int count) {
        pool = new List<GameObject>();

        // Create a number of game objects equal to the count and set them to inactive
        for (int i = 0; i < count; i++) {
            GameObject instance = GameObject.Instantiate(obj);
            instance.SetActive(false);
            pool.Add(instance);
        }
    }

    public GameObject GetObject() {
        // Find the first inactive game object and return it
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                return pool[i];
            }
        }

        // If there are no inactive game objects, create a new one
        GameObject obj = GameObject.Instantiate(pool[0].gameObject);
        pool.Add(obj);
        return obj;
    }
}




}
