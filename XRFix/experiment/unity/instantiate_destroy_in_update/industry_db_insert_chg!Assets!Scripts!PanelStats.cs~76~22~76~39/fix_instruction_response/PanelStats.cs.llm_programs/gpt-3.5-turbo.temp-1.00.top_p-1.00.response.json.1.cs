        // Here's the updated code with Instantiate/Destroy method moved to a more appropriate location:
        private void Start()
        {
            StartCoroutine(SpawnAndDestroyObject());
        }

        private IEnumerator SpawnAndDestroyObject()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeLimit);

                if (!instantiate_gobj)
                {
                    a1 = Instantiate(gobj1, transform.position, Quaternion.identity);
                    instantiate_gobj = true;
                }
                else
                {
                    Destroy(a1);
                    instantiate_gobj = false;
                }
            }
        }

        private void Update()
        {
            rb3.transform.Translate(0, 0, Time.deltaTime * 2);

            var p = marker.localPosition;
            p.y = 30f + 4f * Mathf.Sin(Time.time * 5f);
            marker.localPosition = p;
        }
