

        void Update()
        {

           enemyLifetime -= Time.deltaTime;
           if (enemyLifetime <= 0f) {
              this.gameObject.SetActive(false)
           }
        }
