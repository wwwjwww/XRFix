     void Awake()
     {
            objectPool = new Queue<GameObject>();
            // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
            for (int i = 0; i < 10; i++)
            {
                var pooledObject = Instantiate(enemyPrefab);
                pooledObject.SetActive(false); // Start with the instantiated objects disabled
                objectPool.Enqueue(pooledObject);
            }
     }

     void Update() {


         if (enemyPointer < player.cameraRig.transform.position.z) {
             enemyPointer += spawnDistanceFromEnemy;

             var enemyObject = objectPool.Dequeue();
             enemyObject.SetActive(true);

             enemyObject.transform.position = new Vector3(Random.Range(-4f, 4f), Random.Range(2.5f, 6.5f), player.cameraRig.transform.position.z + spawnDistanceFromPlayer);

         }

         gameTimer += Time.deltaTime;   // Increment the game timer


         if (isGamerOver == false) {
             uiText.text = "Time: " + Mathf.FloorToInt(gameTimer) + "   Score: " + player.score  +  "   Speed: " + Mathf.Floor(player.speed) ;

             if (player.reachedFinishLine == true) { // Change Boolean when player hits finish line and save the final time and score
                 isGamerOver = true;
                 finalTime = gameTimer;
                 finalScore = player.score;
             }
         } else {  // What to do when the player hits the finish line
             uiText.text = "Game Over!\nTime: " + Mathf.FloorToInt(finalTime) + "   Score: " + finalScore + "\nRestarting in: " + Mathf.Floor(gameOverTimer);
             player.speed = 0.3f;  player.maxSpeed = 0.75f;

             gameOverTimer -= Time.deltaTime;    // Gamer restart logic
             if (gameOverTimer <= 0) {
                 SceneManager.LoadScene(SceneManager.GetActiveScene().name);
             }
         }



     }
