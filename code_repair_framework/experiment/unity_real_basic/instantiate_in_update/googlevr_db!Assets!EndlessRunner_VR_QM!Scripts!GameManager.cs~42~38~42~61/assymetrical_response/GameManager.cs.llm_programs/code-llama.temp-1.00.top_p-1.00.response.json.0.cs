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

     


 public void Start () {
        enemyPointer = 10f;
        if isGamerOver == false {
            if enemyPointer < spawnDistanceFromPlayer {
                float newPositionX = transform.position.x + Random.Range (-spawnDistanceFromEnemy, spawnDistanceFromEnemy);
                float newPositionY = transform.position.y + Random.Range (-spawnDistanceFromEnemy, spawnDistanceFromEnemy);
                float newPositionZ = transform.position.z + Random.Range (-spawnDistanceFromEnemy, spawnDistanceFromEnemy);
                Vector3 newPosition = new Vector3 (newPositionX, newPositionY, newPositionZ);
                GameObject newEnemy = GameObject.Instantiate (enemyPrefab, newPosition, Quaternion.identity)
                newEnemy.transform.parent = gameObject.transform;
            }





public GameManager : MonoBehaviour {

 public GameObject GetFinishLine () {
        return finishLine;
    }

 public float GetFinishLinePosition () {
        return finishLinePosition;
    }

 public void SetFinishLinePosition (float position) {
        finishLinePosition = position;
    }

 public void SetEnemyPointer (float pointer) {
        enemyPointer = pointer;
    }

 public void SetPlayer (Player player) {
        this.player = player;
    }}

public void SetEnemyPrefab (GameObject enemy) {
enemyPrefab = enemy;
}

 public GameObject GetEnemyPrefab () {
        return enemyPrefab;
    }

 public float GetSpawnDistanceFromPlayer () {
        return spawnDistanceFromPlayer;
    }

 public void SetSpawnDistanceFromPlayer (float distance) {
        spawnDistanceFromPlayer = distance;
    }

 public float GetSpawnDistanceFromEnemy () {
        return spawnDistanceFromEnemy;
    }

 public void SetSpawnDistanceFromEnemy (float distance) {
        spawnDistanceFromEnemy = distance;
    }

 public void SetUIText (TextMesh ui) {
        uiText = ui;
    }

 public TextMesh GetUIText () {
        return uiText;
    }

 public void SetGameTimer (float timer) {
        gameTimer = timer;
    }

 public float GetGameTimer () {
        return gameTimer;
    }

 public void SetFinalTime (float time) {
        finalTime = time;
    }

 public float GetFinalTime () {
        return finalTime;
    }

 public void SetFinalScore (int score) {
        finalScore = score;
    }

 public int GetFinalScore () {
        return finalScore;
    }

 public bool GetIsGameOver () {
        return isGamerOver;
    }

 public void SetIsGameOver (bool isOver) {
        isGamerOver = isOver;
    }

 public float GetGameOverTimer () {
        return gameOverTimer;
    }

 public void SetGameOverTimer (float timer) {
        gameOverTimer = timer;
    }

public void Update () {
        if isGamerOver == false {
            enemyPointer = (player.transform.position - gameObject.transform.position).magnitude;
        } else {

public void EndGame () {
        if isGamerOver == false {
            isGamerOver = true;
            finalTime = gameTimer;
        }
    }



}
