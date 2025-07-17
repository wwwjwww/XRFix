 	void FixedUpdate () {
 		if (Input.GetButtonDown(button))
         {
             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);

             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
             temp.GetComponent<Launchable>().Player = player;
             temp.GetComponent<Launchable>().button = button;
         }
 	}
