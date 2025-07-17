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
            //         gameTimer += Time.deltaTime;   
            // 
            // 
            //         if (isGamerOver == false) {
            //             uiText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "   Score: " + player.score  +  "   Speed: " + Mathf.Floor(player.speed) ;
            //             
            //             if (player.reachedFinishLine == true) { 
            //                 isGamerOver = true;
            //                 finalTime = gameTimer;
            //                 finalScore = player.score;
            //             }
            //         } else {  
            //             uiText.text = "Game Over!\nTime: " + Mathf.FloorToInt(finalTime) + "   Score: " + finalScore + "\nRestarting in: " + Mathf.Floor(gameOverTimer);
            //             player.speed = 0.3f;  player.maxSpeed = 0.75f;
            // 
            //             gameOverTimer -= Time.deltaTime;    
            //             if (gameOverTimer <= 0) {
            //                 SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
            //             }
            //         }
            // 

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


            GameObject enemyObject = Instantiate(enemyPrefab);




}
