
 	void Update () {

 		if (Mathf.Abs (startingY - transform.position.y) > yDistanceThreshold) {
 			gameObject.SetActive(false);
 		}
 	}
