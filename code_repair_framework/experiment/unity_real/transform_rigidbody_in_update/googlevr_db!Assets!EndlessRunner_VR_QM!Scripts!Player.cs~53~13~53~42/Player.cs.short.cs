
///     void Update()
//     {
//         
//         speed += acceleration * Time.deltaTime;
//         if (speed > maxSpeed) {
//             speed = maxSpeed;   
//         }
// 
//         
//         Vector3 direction = new Vector3( transform.forward.x, 0,  transform.forward.z); 
//         
//         
//         cameraRig.transform.position +=  direction.normalized * speed *  Time.deltaTime;
// 
// 
//         
//         if (transform.position.x <-4.5f) {  
//             transform.position = new Vector3(-4.5f, transform.position.y, transform.position.z); 
//         } else if (transform.position.x > 4.5f) {   
//             transform.position = new Vector3(4.5f, transform.position.y, transform.position.z);
//         }
// 
// 
//         
//         if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("space"))   {
            //             bike.GetComponent<Rigidbody>().AddForce(transform.up * 1000f);
            //             speed -= deceleration;  
            //         }
            // 
            // 
            //         
            //         if (bike.transform.position.x < -4.5f) {   
            //             bike.transform.position = new Vector3(bike.transform.position.x + 2f, bike.transform.position.y + 2, transform.position.z + 4.5f);
            //         } else if (bike.transform.position.x > 4.5f) {   
            //             bike.transform.position = new Vector3(bike.transform.position.x - 2f, bike.transform.position.y + 2, transform.position.z + 4.5f);
            //         }
            // 
            // 
            //         
            //         if (bike.transform.position.z > this.transform.position.z + 12f) {  
            //             bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 11.9f);
            //         } else  if (bike.transform.position.z < this.transform.position.z - 2f) {  
            //             bike.transform.position = new Vector3(bike.transform.position.x, bike.transform.position.y, transform.position.z + 1f);
            //         }
            // 
            // 
            // 
            //         /*
            //         
            //         RaycastHit hit;
            //         if (Physics.Raycast(transform.position, transform.forward, out hit)) {
            //              Debug.Log(hit.transform.name);  
            //          }
            //         */
            //     }

            // FIXED CODE:
